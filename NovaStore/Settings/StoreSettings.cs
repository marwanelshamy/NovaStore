namespace NovaStore.Settings
{
    public class StoreSettings
    {
        public string StoreName { get; set; } = "NOVA";
        public string Currency { get; set; } = "USD";
        public string ContactEmail { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public int LowStockThreshold { get; set; } = 3;
        public SocialLinks SocialLinks { get; set; } = new();
    }

    public class SocialLinks
    {
        public string Instagram { get; set; } = "";
        public string Pinterest { get; set; } = "";
        public string TikTok { get; set; } = "";
        public string Facebook { get; set; } = "";
    }
}