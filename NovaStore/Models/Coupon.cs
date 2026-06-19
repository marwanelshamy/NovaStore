namespace NovaStore.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public decimal DiscountPercent { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Computed — not stored in DB
        public bool IsValid =>
            IsActive &&
            (ExpiryDate == null || ExpiryDate > DateTime.UtcNow) &&
            (UsageLimit == null || UsedCount < UsageLimit);
    }
}