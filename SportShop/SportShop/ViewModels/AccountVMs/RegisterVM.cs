using System.ComponentModel.DataAnnotations;

namespace SportShop.ViewModels.AccountVMs
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Ad və Soyad mütləqdir")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "İstifadəçi adı (Username) mütləqdir")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "İstifadəçi adı 3-50 simvol arasında olmalıdır")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email mütləqdir")]
        [EmailAddress(ErrorMessage = "Düzgün email daxil edin")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifrə mütləqdir")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifrənin təkrarı mütləqdir")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifrələr uyğun gəlmir!")]
        public string ConfirmPassword { get; set; }
    }
}
