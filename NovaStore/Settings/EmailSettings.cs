namespace NovaStore.Settings
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string SenderEmail { get; set; } = "";
        public string SenderName { get; set; } = "";
        public string AppPassword { get; set; } = "";
    }
}