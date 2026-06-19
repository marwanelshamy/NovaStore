namespace NovaStore.ViewModels.Cart
{
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = "";
        public string VariantLabel { get; set; } = "";   // e.g. "Sand / M"
        public string? ImageFileName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
        public int AvailableStock { get; set; }
    }
}