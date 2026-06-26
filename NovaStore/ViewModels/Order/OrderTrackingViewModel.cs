using NovaStore.Models.Enums;

namespace NovaStore.ViewModels.Order
{
    public class OrderTrackingViewModel
    {
        public string OrderNumber { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public OrderStatus CurrentStatus { get; set; }
        public decimal Total { get; set; }
        public List<OrderItemDisplay> Items { get; set; } = new();
        public List<StatusTimelineEntry> Timeline { get; set; } = new();
    }

    public class OrderItemDisplay
    {
        public string ProductName { get; set; } = "";
        public string VariantLabel { get; set; } = "";
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class StatusTimelineEntry
    {
        public OrderStatus Status { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Note { get; set; }
    }
}