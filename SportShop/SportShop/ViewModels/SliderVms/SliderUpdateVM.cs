using System.ComponentModel.DataAnnotations;

namespace SportShop.ViewModels.SliderVms
{
    public class SliderUpdateVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlıq sahəsi vacibdir")]
        [Display(Name = "Əsas Başlıq")]
        public string Title { get; set; }

        [Display(Name = "Alt Başlıq")]
        public string Subtitle { get; set; }

        [Display(Name = "Yönləndirmə Linki (Vacib deyil)")]
        public string? Link { get; set; }

        [Display(Name = "Sıra Nömrəsi")]
        public int Order { get; set; }

        [Display(Name = "Yeni Şəkil Seç (Dəyişmək istəmirsinizsə boş buraxın)")]
        public IFormFile? ImageFile { get; set; } 
        public string? ExistingImageUrl { get; set; }
    }
}
