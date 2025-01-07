namespace eBook_Library_Service.Models
{
    public class ShoppingCartItem
    {
        public int ShoppingCartItemId { get; set; }
        public int BookId { get; set; } // Link to the book
        public Book Book { get; set; } // Navigation property
        public int Quantity { get; set; } = 1; // Default quantity is 1
        public bool IsForBorrow { get; set; } // Indicates if the item is for borrowing or buying
        public int ShoppingCartId { get; set; } // Link to the shopping cart
        public ShoppingCart ShoppingCart { get; set; } // Navigation property
    }
}