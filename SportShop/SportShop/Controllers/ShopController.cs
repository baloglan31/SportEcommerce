using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;
using SportShop.ViewModels;
using SportShop.ViewModels.ProductVMs;
using SportShop.ViewModels.ShopVM;
using System.Security.Claims;

namespace SportShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId, string? searchTerm, string? sortBy)
        {
            var categories = await _context.Categories.ToListAsync();

            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsQueryable();

            
            if (categoryId != null && categoryId > 0)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }

            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
               
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            
            switch (sortBy)
            {
                case "price_asc":
                    productsQuery = productsQuery.OrderBy(p => p.Price); 
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                case "name_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Name); 
                    break;
                case "name_asc":
                    productsQuery = productsQuery.OrderBy(p => p.Name); 
                    break;
                default:
                    productsQuery = productsQuery.OrderByDescending(p => p.Id); 
                    break;
            }


            var products = await productsQuery
        .Select(p => new ProductListVM
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryName = p.Category.Name,

            
            MainImageUrl = p.Images.Where(i => i.IsMain).Select(i => i.Url).FirstOrDefault() ?? "default.jpg"

        }).ToListAsync();


            var model = new ShopVM
            {
                Categories = categories,
                Products = products,
                SelectedCategoryId = categoryId,
                SearchTerm = searchTerm,
                SortBy = sortBy
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews) 
                    .ThenInclude(r => r.AppUser) 
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        [Authorize] // qeydiyyatdan kecmis userler ucun
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            // datalar yoxlanir
            if (rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(comment))
            {
                TempData["Error"] = "Ulduz seçməli və rəy mətni yazmalısınız.";
                return RedirectToAction("Details", new { id = productId });
            }

            // istifadecinin id-si
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Yeni rəy obyekti yaradırıq
            var newReview = new Review
            {
                ProductId = productId,
                AppUserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedDate = DateTime.Now
            };

            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rəyiniz uğurla əlavə edildi!";
            return RedirectToAction("Details", new { id = productId });
        }
    }
}
