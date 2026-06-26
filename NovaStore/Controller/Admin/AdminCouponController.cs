using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaStore.Models;
using NovaStore.Services.Interfaces;
using NovaStore.ViewModels.Admin;

namespace NovaStore.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminCouponController : Controller
    {
        private readonly ICouponService _couponService;

        public AdminCouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            var coupons = await _couponService.GetAllAsync();
            return View(coupons);
        }

        public IActionResult Create()
        {
            return View(new AdminCouponFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCouponFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _couponService.CreateAsync(new Coupon
            {
                Code = model.Code.ToUpper(),
                DiscountPercent = model.DiscountPercent,
                ExpiryDate = model.ExpiryDate,
                UsageLimit = model.UsageLimit
            });

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(int id)
        {
            await _couponService.DisableAsync(id);
            return RedirectToAction("Index");
        }
    }
}