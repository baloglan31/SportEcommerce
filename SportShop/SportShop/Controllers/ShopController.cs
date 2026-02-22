using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.ViewModels;
using SportShop.ViewModels.ProductVMs;

namespace SportShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var categories = await _context.Categories.ToListAsync();

            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsQueryable();


            if (categoryId != null)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }

            var products = await productsQuery
                .OrderByDescending(p => p.Id)
                .Select(p => new ProductListVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    MainImageUrl = p.Images.FirstOrDefault(i => i.IsMain).Url
                }).ToListAsync();

            var model = new ShopVM
            {
                Categories = categories,
                Products = products,
                SelectedCategoryId = categoryId
            };

            return View(model);
        }


        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }
    }
}
