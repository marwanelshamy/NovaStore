using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaStore.Models.Enums;
using NovaStore.Services.Interfaces;
using NovaStore.ViewModels.Admin;

namespace NovaStore.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderService _orderService;

        public AdminOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(string? status)
        {
            var orders = await _orderService.GetAllForAdminAsync(status);
            return View(new AdminOrderListViewModel { Orders = orders, StatusFilter = status });
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            return View(new AdminOrderDetailViewModel { Order = order });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status, string? note)
        {
            await _orderService.UpdateStatusAsync(orderId, status, note);
            return RedirectToAction("Details", new { id = orderId });
        }
    }
}