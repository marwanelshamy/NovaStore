using NovaStore.Services.Interfaces;

namespace NovaStore.Services.Implementations
{
    public class PaymobPaymentService : IPaymentService
    {
        public Task<string> InitiatePaymentAsync(int orderId, decimal amount, string email) =>
            Task.FromResult("");

        public bool VerifyHmac(string payload, string hmacHeader) => true;
    }
}