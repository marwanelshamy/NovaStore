using NovaStore.Models.Enums;

namespace NovaStore.Models
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; }

        // Foreign Key
        public int OrderId { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
    }
}