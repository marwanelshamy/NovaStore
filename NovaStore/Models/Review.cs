namespace NovaStore.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }          // 1 to 5
        public string? Comment { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int ProductId { get; set; }
        public string UserId { get; set; } = "";

        // Navigation
        public Product Product { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}