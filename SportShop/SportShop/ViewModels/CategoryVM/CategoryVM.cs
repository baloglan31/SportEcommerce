using System.ComponentModel.DataAnnotations;

namespace SportShop.ViewModels.CategoryVM
{
    public class CategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kateqoriya adı mütləq yazılmalıdır")]
        [StringLength(50, ErrorMessage = "Maksimum 50 simvol ola bilər")]
        [Display(Name = "Kateqoriya Adı")]
        public string Name { get; set; }
    }
}
