using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaStore.Services.Interfaces;

namespace NovaStore.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IInventoryService _inventoryService;

        public AdminDashboardController(IOrderService orderService, IInventoryService inventoryService)
        {
            _orderService = orderService;
            _inventoryService = inventoryService;
        }

        public async Task<IActionResult> Index()
        {
            var lowStock = await _inventoryService.GetLowStockVariantsAsync(3);

            ViewBag.LowStockCount = lowStock.Count();
            ViewBag.LowStockItems = lowStock;

            return View();
        }
    }
}