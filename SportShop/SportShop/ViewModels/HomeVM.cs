using SportShop.Models;
using SportShop.ViewModels.ProductVMs;

namespace SportShop.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public List<Category> Categories { get; set; }
        public List<ProductListVM> FeaturedProducts { get; set; }
    }
}
