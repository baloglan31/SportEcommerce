using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.ViewModels.DashboardVM;
using SportShop.ViewModels.ProductVMs;

namespace SportShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {

        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            
            int productCount = await _context.Products.CountAsync();
            int categoryCount = await _context.Categories.CountAsync();
            int sliderCount = await _context.Sliders.CountAsync();


            var recentProducts = await _context.Products
        .Include(p => p.Category)
        .Include(p => p.Images)
        .OrderByDescending(p => p.Id)
        .Take(5)
        .Select(p => new ProductListVM
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryName = p.Category.Name,
            MainImageUrl = p.Images.FirstOrDefault(i => i.IsMain).Url
        })
        .ToListAsync();

            var model = new DashboardVM
            {
                ProductCount = productCount,
                CategoryCount = categoryCount,
                SliderCount = sliderCount, 
                RecentProducts = recentProducts
            };

            return View(model);
        }
    }
}
