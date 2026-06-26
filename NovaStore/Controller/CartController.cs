using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovaStore.Models;
using NovaStore.Services.Implementations;
using NovaStore.Services.Interfaces;

namespace NovaStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartService cartService, ICouponService couponService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _couponService = couponService;
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
            var coupon = CartHelper.GetCoupon(HttpContext);
            var cart = await _cartService.GetCartAsync(userId, sessionId, coupon);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int variantId, int quantity = 1)
        {
            var (userId, sessionId) = await GetIdentity();
            await _cartService.AddItemAsync(variantId, quantity, userId, sessionId);

            var cart = await _cartService.GetCartAsync(userId, sessionId);
            return Json(new { success = true, itemCount = cart.ItemCount });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQty(int cartItemId, int quantity)
        {
            var (userId, _) = await GetIdentity();
            await _cartService.UpdateQtyAsync(cartItemId, quantity, userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var (userId, _) = await GetIdentity();
            await _cartService.RemoveItemAsync(cartItemId, userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            var (userId, sessionId) = await GetIdentity();
            var cart = await _cartService.GetCartAsync(userId, sessionId);

            var coupon = await _couponService.ValidateAsync(couponCode);

            if (coupon != null)
            {
                CartHelper.SetCoupon(HttpContext, couponCode);
                var discount = cart.Subtotal * (coupon.DiscountPercent / 100m);
                TempData["CouponMessage"] = $"✓ Coupon applied! {couponCode.ToUpper()} — {coupon.DiscountPercent}% off";
            }
            else
            {
                CartHelper.SetCoupon(HttpContext, null);
                TempData["CouponMessage"] = "Invalid or expired coupon code.";
            }

            return RedirectToAction("Index");
        }
    }
}