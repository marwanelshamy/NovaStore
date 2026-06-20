using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NovaStore.Services.Interfaces;
using NovaStore.Settings;

namespace NovaStore.Services.Implementations
{
    public class PaymobPaymentService : IPaymentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PaymobSettings _settings;
        private readonly ILogger<PaymobPaymentService> _logger;

        public PaymobPaymentService(
            IHttpClientFactory httpClientFactory,
            IOptions<PaymobSettings> settings,
            ILogger<PaymobPaymentService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<string> InitiatePaymentAsync(int orderId, decimal amount, string email)
        {
            var client = _httpClientFactory.CreateClient("Paymob");

            // ── Step 1: Authenticate — get a temporary auth token ──
            var authResponse = await client.PostAsync("auth/tokens", JsonContent(new
            {
                api_key = _settings.ApiKey
            }));
            var authJson = JObject.Parse(await authResponse.Content.ReadAsStringAsync());
            var authToken = authJson["token"]?.ToString();

            // ── Step 2: Register the order with Paymob ──
            var amountCents = (int)(amount * 100); // Paymob expects amount in cents
            var orderResponse = await client.PostAsync("ecommerce/orders", JsonContent(new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = amountCents,
                currency = "EGP",
                merchant_order_id = orderId.ToString(),
                items = Array.Empty<object>()
            }));
            var orderJson = JObject.Parse(await orderResponse.Content.ReadAsStringAsync());
            var paymobOrderId = orderJson["id"]?.ToString();

            // ── Step 3: Request a payment key tied to this order ──
            var billingData = new
            {
                apartment = "NA",
                email = email,
                floor = "NA",
                first_name = "Customer",
                street = "NA",
                building = "NA",
                phone_number = "+201000000000",
                shipping_method = "NA",
                postal_code = "NA",
                city = "NA",
                country = "EG",
                last_name = "Customer",
                state = "NA"
            };

            var paymentKeyResponse = await client.PostAsync("acceptance/payment_keys", JsonContent(new
            {
                auth_token = authToken,
                amount_cents = amountCents,
                expiration = 3600,
                order_id = paymobOrderId,
                billing_data = billingData,
                currency = "EGP",
                integration_id = _settings.IntegrationId_Card
            }));
            var paymentKeyJson = JObject.Parse(await paymentKeyResponse.Content.ReadAsStringAsync());
            var paymentToken = paymentKeyJson["token"]?.ToString();

            // ── Step 4: Build the redirect URL to Paymob's hosted payment page ──
            var iframeId = _settings.ApiKey; // placeholder — real iframe ID comes from Paymob dashboard
            return $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentToken}";
        }

        public bool VerifyHmac(string payload, string receivedHmac)
        {
            if (string.IsNullOrEmpty(_settings.HmacSecret))
            {
                _logger.LogWarning("Paymob HMAC secret is not configured.");
                return false;
            }

            var keyBytes = Encoding.UTF8.GetBytes(_settings.HmacSecret);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(payloadBytes);
            var computedHmac = Convert.ToHexString(hashBytes).ToLower();

            return computedHmac == receivedHmac.ToLower();
        }

        private static StringContent JsonContent(object obj) =>
            new(Newtonsoft.Json.JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
    }
}