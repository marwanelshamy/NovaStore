namespace NovaStore.ViewModels.Checkout
{
    public class OrderConfirmationViewModel
    {
        public string OrderNumber { get; set; } = "";
        public DateTime PlacedDate { get; set; }
        public string PaymentMethod { get; set; } = "";
        public decimal Total { get; set; }
        public string ShippingName { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public string ShippingCity { get; set; } = "";
        public string ShippingCountry { get; set; } = "";
    }
}