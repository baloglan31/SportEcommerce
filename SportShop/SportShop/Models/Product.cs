using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public List<ProductImage> Images { get; set; } = [];

        public double AverageRating { get; set; } = 0;

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<Review> Reviews { get; set; }
    }
}
