namespace NovaStore.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
        Task SendOrderConfirmationAsync(
            string customerEmail,
            string customerName,
            int orderId,
            decimal total);
    }
}