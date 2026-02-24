using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;
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

       
        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }


        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartItems();
            var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCartItems(cart);
            }

            return RedirectToAction("Index");
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

        [HttpPost]
        public IActionResult UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            if (item != null)
            {
                item.Quantity += request.Change;

                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }

                SaveCartItems(cart);
            }


            int totalItems = cart.Sum(c => c.Quantity);
            decimal cartTotal = cart.Sum(c => c.TotalPrice);
            decimal itemTotal = item != null ? item.TotalPrice : 0;
            int currentQuantity = item != null ? item.Quantity : 0;


            return Json(new
            {
                success = true,
                cartCount = totalItems,
                itemTotal = itemTotal.ToString("C"), 
                cartTotal = cartTotal.ToString("C"),
                currentQuantity = currentQuantity
            });
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCartItems();


            if (cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var vm = new CheckoutVM
            {
                CartItems = cart,
                CartTotal = cart.Sum(c => c.TotalPrice)
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            var cart = GetCartItems();

            if (cart.Count == 0) return RedirectToAction("Index");


            var order = new Order
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(c => c.TotalPrice),
                OrderItems = cart.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Price = c.Price,
                    Quantity = c.Quantity
                }).ToList()
            };


            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Success");
        }


        public IActionResult Success()
        {
            return View();
        }
    }
}
