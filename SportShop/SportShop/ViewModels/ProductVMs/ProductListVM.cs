namespace SportShop.ViewModels.ProductVMs
{
    public class ProductListVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public string MainImageUrl { get; set; }
        public List<string> AdditionalImageUrls { get; set; } = new List<string>();
    }
}

