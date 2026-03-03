namespace SportShop.ViewModels.CartVMs
{
    public class CheckoutVM
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public List<CartItemVM>? CartItems { get; set; }
        public decimal CartTotal { get; set; }

    }
}
