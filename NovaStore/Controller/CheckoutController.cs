using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NovaStore.Models;
using NovaStore.Services.Implementations;
using NovaStore.Services.Interfaces;
using NovaStore.ViewModels.Checkout;

namespace NovaStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        public CheckoutController(
     ICartService cartService,
     IOrderService orderService,
     IPaymentService paymentService,
     UserManager<ApplicationUser> userManager,
     IEmailService emailService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _paymentService = paymentService;
            _userManager = userManager;
            _emailService = emailService;
        }

        private async Task<(string? userId, string? sessionId)> GetIdentity()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                return (user?.Id, null);
            }
            return (null, CartHelper.GetOrCreateSessionId(HttpContext));
        }

        public async Task<IActionResult> Index()
        {
            var (userId, sessionId) = await GetIdentity();
            var coupon = CartHelper.GetCoupon(HttpContext);
            var cart = await _cartService.GetCartAsync(userId, sessionId, coupon);

            if (!cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            var model = new CheckoutViewModel { Cart = cart };

            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    model.Email = user.Email ?? "";
                    model.FullName = user.FullName;
                    model.Phone = user.Phone ?? "";
                }
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCallback([FromBody] Newtonsoft.Json.Linq.JObject payload, [FromQuery] string hmac)
        {
            var rawPayload = payload.ToString(Newtonsoft.Json.Formatting.None);

            if (!_paymentService.VerifyHmac(rawPayload, hmac))
            {
                return BadRequest("Invalid HMAC signature.");
            }

            var success = payload["obj"]?["success"]?.Value<bool>() ?? false;
            var merchantOrderId = payload["obj"]?["order"]?["merchant_order_id"]?.ToString();

            if (success && int.TryParse(merchantOrderId, out var orderId))
            {
                await _orderService.ConfirmOrderAsync(orderId);
            }

            return Ok();
        }

        public async Task<IActionResult> Confirmation(string orderNumber)
        {
            var order = await _orderService.GetByNumberAsync(orderNumber);
            if (order == null) return NotFound();

            var model = new OrderConfirmationViewModel
            {
                OrderNumber = order.OrderNumber,
                PlacedDate = order.CreatedAt,
                PaymentMethod = order.PaymentMethod.ToString(),
                Total = order.Total,
                ShippingName = order.ShippingName,
                ShippingAddress = order.ShippingAddress,
                ShippingCity = order.ShippingCity,
                ShippingCountry = order.ShippingCountry
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var (userId, sessionId) = await GetIdentity();
            var coupon = CartHelper.GetCoupon(HttpContext);
            var cart = await _cartService.GetCartAsync(userId, sessionId, coupon);
            if (!cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
            {
                model.Cart = cart;
                return View("Index", model);
            }

            var order = await _orderService.CreateOrderAsync(model, userId, sessionId);

            if (model.PaymentMethod == NovaStore.Models.Enums.PaymentMethod.COD)
            {
                await _emailService.SendOrderConfirmationAsync(
                    order.ShippingEmail,
                    order.ShippingName,
                    order.Id,
                    order.Total
                );

                return RedirectToAction("Confirmation",
                    new { orderNumber = order.OrderNumber });
            }
            else
            {
                var redirectUrl = await _paymentService.InitiatePaymentAsync(
                    order.Id,
                    order.Total,
                    model.Email);

                return Redirect(redirectUrl);
            }
        }


    }
}