using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.ViewModels;
using System.Text.Json;

namespace SportShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId);

            if (product == null)
            {
                return Json(new { success = false, message = "Məhsul tapılmadı!" });
            }

            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Add(new CartItemVM
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = request.Quantity,
                    ImageUrl = product.Images.FirstOrDefault(i => i.IsMain)?.Url ?? "/assets/img/no-image.png"
                });
            }

            SaveCartItems(cart);

            int totalItems = cart.Sum(c => c.Quantity);

            return Json(new { success = true, message = "Məhsul səbətə əlavə edildi!", cartCount = totalItems });
        }

        private List<CartItemVM> GetCartItems()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(sessionCart)
                ? new List<CartItemVM>()
                : JsonSerializer.Deserialize<List<CartItemVM>>(sessionCart);
        }

        private void SaveCartItems(List<CartItemVM> cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }
    }
}
