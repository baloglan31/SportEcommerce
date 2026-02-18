using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;
using SportShop.ViewModels.ProductVMs;

namespace SportShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Select(p => new ProductListVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    MainImageUrl = p.Images.FirstOrDefault(i => i.IsMain).Url,
                    AdditionalImageUrls = p.Images.Where(i => !i.IsMain).Select(i => i.Url).ToList()
                })
                .ToListAsync();

            return View(products);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    Images = new List<ProductImage>()
                };

            
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img/products");

             
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                if (model.MainImage != null)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.MainImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.MainImage.CopyToAsync(fileStream);
                    }

                    product.Images.Add(new ProductImage
                    {
                     
                        Url = "/assets/img/products/" + uniqueFileName,
                        IsMain = true
                    });
                }

              
                if (model.AdditionalImages != null)
                {
                    foreach (var file in model.AdditionalImages)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        product.Images.Add(new ProductImage
                        {
                           
                            Url = "/assets/img/products/" + uniqueFileName,
                            IsMain = false
                        });
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();


            var model = new UpdateProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,


                ExistingMainImageUrl = product.Images.FirstOrDefault(i => i.IsMain)?.Url,
                ExistingAdditionalImages = product.Images.Where(i => !i.IsMain).ToList()
            };

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProductVM model)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == model.Id);

                if (product == null) return NotFound();

                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

             
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img/products");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        
                if (model.MainImage != null)
                {
                  
                    var oldMain = product.Images.FirstOrDefault(i => i.IsMain);
                    if (oldMain != null)
                    {

                        string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, oldMain.Url.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);

                        _context.ProductImages.Remove(oldMain);
                    }

                
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.MainImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.MainImage.CopyToAsync(fileStream);
                    }

                    product.Images.Add(new ProductImage
                    {
                        Url = "/assets/img/products/" + uniqueFileName, 
                        IsMain = true
                    });
                }

               
                if (model.AdditionalImages != null)
                {
                    foreach (var file in model.AdditionalImages)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        product.Images.Add(new ProductImage
                        {
                            Url = "/assets/img/products/" + uniqueFileName, 
                            IsMain = false
                        });
                    }
                }

                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId, int productId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.Url.TrimStart('/'));
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error deleting image file: {ex.Message}");
                }

                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), new { id = productId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);

            if (image == null) return NotFound();

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.Url.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = image.ProductId });
        }
    }
}