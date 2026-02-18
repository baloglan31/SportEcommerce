using System.ComponentModel.DataAnnotations;
using SportShop.Models;

namespace SportShop.ViewModels.ProductVMs
{
    public class UpdateProductVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Məhsul adı boş ola bilməz")]
        [Display(Name = "Məhsul Adı")]
        public string Name { get; set; }

        [Display(Name = "Təsvir")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Qiymət yazılmalıdır")]
        [Display(Name = "Qiymət")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Kateqoriya seçilməlidir")]
        [Display(Name = "Kateqoriya")]
        public int CategoryId { get; set; }

        [Display(Name = "Əsas Şəkli Dəyiş (Seçilməsə köhnəsi qalacaq)")]
        public IFormFile? MainImage { get; set; }

        [Display(Name = "Yeni Əlavə Şəkillər Yüklə")]
        public List<IFormFile>? AdditionalImages { get; set; }

        public string? ExistingMainImageUrl { get; set; }
        public List<ProductImage>? ExistingAdditionalImages { get; set; }
    }
}

