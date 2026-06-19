namespace NovaStore.Settings
{
    public class PaymobSettings
    {
        public string ApiKey { get; set; } = "";
        public string IntegrationId_Card { get; set; } = "";
        public string IntegrationId_Wallet { get; set; } = "";
        public string HmacSecret { get; set; } = "";
    }
}