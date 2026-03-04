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

        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, string? sortBy, int page = 1)
        {
            int pageSize = 12; 

            
            var query = _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
               
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

           
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

          
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "date_asc" => query.OrderBy(p => p.Id), 
                _ => query.OrderByDescending(p => p.Id) 
            };

           
            int totalProducts = await query.CountAsync(); 
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize); 

           
            if (page > totalPages && totalPages > 0) page = totalPages;
            if (page < 1) page = 1;

           
            var products = await query
                .Skip((page - 1) * pageSize) 
                .Take(pageSize) 
                .ToListAsync();

            var categories = await _context.Categories.ToListAsync();

            
            var productVMs = products.Select(p => new ProductListVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category?.Name ?? string.Empty,
                MainImageUrl = p.Images != null && p.Images.Count > 0 ? p.Images[0].Url : string.Empty,
                AdditionalImageUrls = p.Images != null && p.Images.Count > 1
                    ? p.Images.Skip(1).Select(img => img.Url).ToList()
                    : new List<string>()
            }).ToList();

            
            var vm = new ShopVM
            {
                Products = productVMs,
                Categories = categories,
                SearchTerm = searchTerm,
                SelectedCategoryId = categoryId,
                SortBy = string.IsNullOrEmpty(sortBy) ? "date_desc" : sortBy,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(vm);
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
