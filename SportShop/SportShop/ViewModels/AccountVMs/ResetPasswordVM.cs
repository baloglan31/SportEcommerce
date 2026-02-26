using System.ComponentModel.DataAnnotations;

namespace SportShop.ViewModels.AccountVMs
{
    public class ResetPasswordVM
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Yeni şifrə mütləqdir")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifrənin təkrarı mütləqdir")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifrələr uyğun gəlmir!")]
        public string ConfirmPassword { get; set; }
    }
}
