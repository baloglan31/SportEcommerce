using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SportShop.Data;
using SportShop.Models;
using SportShop.Services;
using SportShop.ViewModels.AccountVMs;

namespace SportShop.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context, EmailService emailService) : Controller
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly ApplicationDbContext _context = context;
        private readonly EmailService _emailService = emailService;

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);


            AppUser user = new AppUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.UserName
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
               
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                
                string emailBody = $"<h3>Xoş gəlmisiniz!</h3><p>Hesabınızı aktivləşdirmək üçün zəhmət olmasa <a href='{confirmationLink}'>BU LİNKƏ KLİKLƏYİN</a>.</p>";
                await _emailService.SendEmailAsync(user.Email, "SportShop - Hesabın Təsdiqi", emailBody);

                
                return RedirectToAction("CheckEmail");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            var model = new LoginVM { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "İstifadəçi adı və ya Şifrə yalnışdır!");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "İstifadəçi adı və ya Şifrə yalnışdır!");
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Orders()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");


            var myOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .Where(o => o.AppUserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(myOrders);
        }
        
        public IActionResult CheckEmail()
        {
            return View();
        }

      
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("İstifadəçi tapılmadı");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View("ConfirmEmailSuccess");
            }

            return Content("Təsdiqləmə zamanı xəta baş verdi. Linkin vaxtı keçmiş ola bilər.");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return RedirectToAction("ForgotPasswordConfirmation");
            }


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            var callbackUrl = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);

            
            string body = $"<h3>Şifrə Yeniləmə</h3><p>Hesabınızın şifrəsini yeniləmək üçün zəhmət olmasa <a href='{callbackUrl}'>BU LİNKƏ KLİKLƏYİN</a>.</p>";
            await _emailService.SendEmailAsync(model.Email, "SportShop - Şifrənin Sıfırlanması", body);

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

       
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null) return RedirectToAction("Index", "Home");

            var model = new ResetPasswordVM { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return RedirectToAction("ResetPasswordConfirmation");

            
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
