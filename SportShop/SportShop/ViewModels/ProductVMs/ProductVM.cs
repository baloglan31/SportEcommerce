using System.ComponentModel.DataAnnotations;

namespace SportShop.ViewModels.ProductVMs
{
    public class ProductVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Məhsul adı vacibdir")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Display(Name = "Əsas Şəkil (Vitrin)")]
        public IFormFile MainImage { get; set; }

        [Display(Name = "Əlavə Şəkillər (Qalereya)")]
        public List<IFormFile>? AdditionalImages { get; set; }
    }
}
