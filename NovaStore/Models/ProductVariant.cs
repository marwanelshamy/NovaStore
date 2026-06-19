namespace NovaStore.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public string Size { get; set; } = "";
        public string Color { get; set; } = "";
        public string SKU { get; set; } = "";
        public int StockQuantity { get; set; } = 0;
        public bool IsAvailable => StockQuantity > 0;

        // Foreign Key
        public int ProductId { get; set; }

        // Navigation
        public Product Product { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}