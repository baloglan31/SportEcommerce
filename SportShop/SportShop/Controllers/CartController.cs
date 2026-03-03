using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;
using SportShop.ViewModels.CartVMs;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text.Json;



namespace SportShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager; 

        
        public CartController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager; 
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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout() 
        {
            var cart = GetCartItems();
            if (cart.Count == 0) return RedirectToAction("Index");

            
            var user = await _userManager.GetUserAsync(User);

            var vm = new CheckoutVM
            {
                CartItems = cart,
                CartTotal = cart.Sum(c => c.TotalPrice),

                
                FullName = user?.FullName,
                Email = user?.Email,
                Address = user?.Address 
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            // 1. Səbəti yoxlayırıq (Əgər boşdursa və ya yaddaşdan siliniblərsə, səbət səhifəsinə qaytarırıq)
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return RedirectToAction("Index", "Cart");
            }

            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItemVM>>(cartJson);

            // 2. Sifarişi "Pending" statusu ilə bazaya yazırıq
            var order = new Order
            {
                AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonim",
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                TotalAmount = cartItems.Sum(x => x.Price * x.Quantity)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Sifarişin məhsullarını əlavə edirik
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    OrderId = order.Id,
                    Price = item.Price,
                    Quantity = item.Quantity
                };
                _context.OrderItems.Add(orderItem);
            }
            await _context.SaveChangesAsync();

            // 3. Stripe Ödəniş Sessiyasının Yaradılması
            var domain = "https://localhost:7265/"; // Öz işləyən portunuzu burada saxlayın

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Cart/OrderConfirmation?id={order.Id}",
                CancelUrl = domain + "Cart/Index",
            };

            foreach (var item in cartItems)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "azn",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName
                        },
                    },
                    Quantity = item.Quantity,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            // 4. Müştərini Stripe səhifəsinə yönləndiririk
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(u => u.Id == id);

            if (order == null) return NotFound();

            // ÖDƏNİŞ UĞURLUDUR! Statusu dəyişirik:
            order.OrderStatus = "Approved";
            await _context.SaveChangesAsync();

            // Səbəti təmizləyirik
            HttpContext.Session.Remove("Cart");

            return View(id);
        }
    }
}
