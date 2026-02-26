using System.ComponentModel.DataAnnotations;

namespace SportShop.ViewModels.AccountVMs
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Email mütləqdir")]
        [EmailAddress(ErrorMessage = "Düzgün email daxil edin")]
        public string Email { get; set; }
    }
}
