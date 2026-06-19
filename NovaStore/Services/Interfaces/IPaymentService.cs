namespace NovaStore.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> InitiatePaymentAsync(int orderId, decimal amount, string email);
        bool VerifyHmac(string payload, string hmacHeader);
    }
}