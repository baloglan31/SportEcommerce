using System.ComponentModel.DataAnnotations;

namespace SportShop.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } // komment

        [Range(1, 5)]
        public int Rating { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
