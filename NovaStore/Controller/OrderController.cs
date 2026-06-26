using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovaStore.Models;
using NovaStore.Services.Interfaces;
using NovaStore.ViewModels.Order;

namespace NovaStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var orders = await _orderService.GetByUserAsync(user.Id);

            var model = new MyOrdersViewModel
            {
                Orders = orders.Select(o => new OrderSummaryViewModel
                {
                    OrderNumber = o.OrderNumber,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    Total = o.Total,
                    ItemCount = o.Items.Sum(i => i.Quantity)
                }).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Track(string orderNumber)
        {
            var order = await _orderService.GetByNumberAsync(orderNumber);
            if (order == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            // Security: only let a customer view their own order
            if (order.UserId != user?.Id)
                return Forbid();

            var model = new OrderTrackingViewModel
            {
                OrderNumber = order.OrderNumber,
                CreatedAt = order.CreatedAt,
                CurrentStatus = order.Status,
                Total = order.Total,
                Items = order.Items.Select(i => new OrderItemDisplay
                {
                    ProductName = i.ProductName,
                    VariantLabel = i.VariantLabel,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                }).ToList(),
                Timeline = order.StatusHistory
                    .OrderBy(h => h.ChangedAt)
                    .Select(h => new StatusTimelineEntry
                    {
                        Status = h.Status,
                        ChangedAt = h.ChangedAt,
                        Note = h.Note
                    }).ToList()
            };

            return View(model);
        }
    }
}