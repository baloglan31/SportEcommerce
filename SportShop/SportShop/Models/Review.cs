using System.ComponentModel.DataAnnotations;

namespace SportShop.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } 

        [Required, MaxLength(500)]
        public string Comment { get; set; } 

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
