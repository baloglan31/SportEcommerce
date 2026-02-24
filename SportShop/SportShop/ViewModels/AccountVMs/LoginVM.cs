using System.ComponentModel.DataAnnotations;

namespace SportShop.ViewModels.AccountVMs
{
    public class LoginVM
    {
        [Required(ErrorMessage = "İstifadəçi adı mütləqdir")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Şifrə mütləqdir")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
