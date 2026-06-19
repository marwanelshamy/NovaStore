namespace NovaStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Guest or logged-in user
        public string? SessionId { get; set; }
        public string? UserId { get; set; }

        // Foreign Key
        public int ProductVariantId { get; set; }

        // Navigation
        public ProductVariant ProductVariant { get; set; } = null!;
        public ApplicationUser? User { get; set; }
    }
}