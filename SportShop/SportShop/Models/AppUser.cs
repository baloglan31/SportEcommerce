using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SportShop.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        public string FullName { get; set; }

        public string? Address { get; set; } // fake address

        // reyler
        public List<Review> Reviews { get; set; }
    }
}
