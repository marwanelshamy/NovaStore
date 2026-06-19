using NovaStore.Models.Enums;

namespace NovaStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Customer — null if guest
        public string? UserId { get; set; }
        public string? GuestEmail { get; set; }

        // Status
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        // Paymob (only for online payments)
        public string? PaymobOrderId { get; set; }
        public string? PaymobTransactionId { get; set; }

        // Totals
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }

        // Shipping info
        public string ShippingName { get; set; } = "";
        public string ShippingEmail { get; set; } = "";
        public string ShippingPhone { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public string ShippingCity { get; set; } = "";
        public string ShippingCountry { get; set; } = "";
        public string? Notes { get; set; }

        // Navigation
        public ApplicationUser? User { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
}