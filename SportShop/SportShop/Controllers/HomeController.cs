using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;
using SportShop.ViewModels;
using SportShop.ViewModels.ProductVMs;
using System.Diagnostics;

namespace SportShop.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var sliders = await _context.Sliders
                .OrderByDescending(s => s.Id)
                .Take(3)
                .ToListAsync();


            var categories = await _context.Categories
                .Take(3)
                .ToListAsync();


            var featuredProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .OrderByDescending(p => p.Id)
                .Take(3)
                .Select(p => new ProductListVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    MainImageUrl = p.Images.FirstOrDefault(i => i.IsMain).Url
                })
                .ToListAsync();


            var model = new HomeVM
            {
                Sliders = sliders,
                Categories = categories,
                FeaturedProducts = featuredProducts
            };

            return View(model);


        }
    }
}
