namespace NovaStore.ViewModels.Cart
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal Subtotal => Items.Sum(i => i.LineTotal);
        public decimal Tax => Subtotal * 0.10m;
        public decimal Discount { get; set; } = 0;
        public decimal Total => Subtotal + Tax - Discount;
        public int ItemCount => Items.Sum(i => i.Quantity);
        public string? CouponCode { get; set; }
        public bool CouponApplied => Discount > 0;
    }
}