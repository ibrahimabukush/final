namespace eBook_Library_Service.Models
{
    public class ShoppingCart
    {
        public int ShoppingCartId { get; set; }
        public string UserId { get; set; } // Link to the user who owns the cart
        public Users User { get; set; } // Navigation property
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>(); // List of items in the cart
    }
}