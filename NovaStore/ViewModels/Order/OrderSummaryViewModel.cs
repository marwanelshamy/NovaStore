using NovaStore.Models.Enums;

namespace NovaStore.ViewModels.Order
{
    public class OrderSummaryViewModel
    {
        public string OrderNumber { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
    }
}