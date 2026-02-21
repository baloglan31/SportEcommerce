using System.ComponentModel.DataAnnotations;

namespace SportShop.ViewModels.SliderVms
{
    public class SliderCreateVM
    {
        [Required(ErrorMessage = "Başlıq sahəsi vacibdir")]
        [Display(Name = "Əsas Başlıq")]
        public string Title { get; set; }

        [Display(Name = "Alt Başlıq")]
        public string Subtitle { get; set; }

        [Display(Name = "Yönləndirmə Linki (Vacib deyil)")]
        public string? Link { get; set; }

        [Display(Name = "Sıra Nömrəsi")]
        public int Order { get; set; }

        [Required(ErrorMessage = "Zəhmət olmasa bir şəkil seçin")]
        [Display(Name = "Slider Şəkli")]
        public IFormFile ImageFile { get; set; }
    }
}
