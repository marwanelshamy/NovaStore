namespace NovaStore.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        // Snapshot of product at time of purchase
        // (in case product name/price changes later)
        public string ProductName { get; set; } = "";
        public string VariantLabel { get; set; } = "";   // e.g. "Sand / M"
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }

        // Foreign Keys
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int ProductVariantId { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}