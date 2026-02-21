using SportShop.ViewModels.ProductVMs;

namespace SportShop.ViewModels.DashboardVM
{
    public class DashboardVM
    {
        public int ProductCount { get; set; }      
        public int CategoryCount { get; set; }
        public int SliderCount { get; set; }
        public List<ProductListVM> RecentProducts { get; set; }
    }
}
