using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(
            ICartService cartService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userManager = userManager;
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
            var cart = await _cartService.GetCartAsync(userId, sessionId);

            if (!cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            var model = new CheckoutViewModel { Cart = cart };

            // Pre-fill email if logged in
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var (userId, sessionId) = await GetIdentity();
            var cart = await _cartService.GetCartAsync(userId, sessionId);

            if (!cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
            {
                model.Cart = cart;
                return View("Index", model);
            }

            var order = await _orderService.CreateOrderAsync(model, userId, sessionId);

            // Online payment will be wired in the next step (Paymob)
            // For now, COD is the only fully working path

            return RedirectToAction("Confirmation", new { orderNumber = order.OrderNumber });
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
    }
}