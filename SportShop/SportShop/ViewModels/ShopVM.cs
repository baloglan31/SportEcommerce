using SportShop.Models;
using SportShop.ViewModels.ProductVMs;

namespace SportShop.ViewModels
{
    public class ShopVM
    {
        public List<Category> Categories { get; set; }
        public List<ProductListVM> Products { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
}
