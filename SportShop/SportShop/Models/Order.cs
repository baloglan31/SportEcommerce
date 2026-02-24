namespace SportShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        //sifarisin icindeki mehsullar
        public List<OrderItem> OrderItems { get; set; }
    }
}
