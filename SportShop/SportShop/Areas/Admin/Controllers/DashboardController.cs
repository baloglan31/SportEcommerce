using Microsoft.AspNetCore.Mvc;
using SportShop.Data;

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

        public IActionResult Index()
        {

            ViewBag.ProductCount = _context.Products.Count();
            ViewBag.CategoryCount = _context.Categories.Count();
            ViewBag.ReviewCount = _context.Reviews.Count();
            // OrderCount - gələcəkdə əlavə edəcəksiniz

            return View();
        }
    }
}
