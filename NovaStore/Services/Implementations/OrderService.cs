using Microsoft.EntityFrameworkCore;
using NovaStore.Data;
using NovaStore.Models;
using NovaStore.Models.Enums;
using NovaStore.Services.Interfaces;
using NovaStore.ViewModels.Checkout;

namespace NovaStore.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _db;
        private readonly IInventoryService _inventoryService;
        private readonly IEmailService _emailService;
        private readonly ICartService _cartService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            ApplicationDbContext db,
            IInventoryService inventoryService,
            IEmailService emailService,
            ICartService cartService,
            ILogger<OrderService> logger)
        {
            _db = db;
            _inventoryService = inventoryService;
            _emailService = emailService;
            _cartService = cartService;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(CheckoutViewModel model, string? userId, string? sessionId)
        {
            var cart = await _cartService.GetCartAsync(userId, sessionId);

            if (!cart.Items.Any())
                throw new InvalidOperationException("Cannot create an order with an empty cart.");

            var orderNumber = $"NV-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(100000, 999999)}";

            var order = new Order
            {
                OrderNumber = orderNumber,
                UserId = userId,
                GuestEmail = userId == null ? model.Email : null,
                Status = OrderStatus.Pending,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                Subtotal = cart.Subtotal,
                Tax = cart.Tax,
                Discount = cart.Discount,
                Total = cart.Total,
                ShippingName = model.FullName,
                ShippingEmail = model.Email,
                ShippingPhone = model.Phone,
                ShippingAddress = model.Address,
                ShippingCity = model.City,
                ShippingCountry = model.Country,
                Notes = model.Notes,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = GetProductIdFromVariant(i.ProductVariantId),
                    ProductVariantId = i.ProductVariantId,
                    ProductName = i.ProductName,
                    VariantLabel = i.VariantLabel,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                }).ToList()
            };

            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = OrderStatus.Pending,
                Note = "Order placed"
            });

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Reduce stock for each item
            foreach (var item in cart.Items)
            {
                await _inventoryService.ReduceStockAsync(item.ProductVariantId, item.Quantity);
            }

            // Clear the cart now that the order exists
            await _cartService.ClearAsync(userId, sessionId);

            // COD orders are confirmed immediately; online orders wait for payment callback
            if (model.PaymentMethod == PaymentMethod.COD)
            {
                await ConfirmOrderAsync(order.Id);
            }

            return order;
        }

        public async Task ConfirmOrderAsync(int orderId)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return;

            if (order.PaymentMethod != PaymentMethod.COD)
            {
                order.PaymentStatus = PaymentStatus.Paid;
            }

            order.Status = OrderStatus.Confirmed;
            _db.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = OrderStatus.Confirmed,
                Note = "Order confirmed"
            });

            await _db.SaveChangesAsync();

            // Send confirmation email — failure here should not break the order
            try
            {
                var body = BuildConfirmationEmail(order);
                var toEmail = order.GuestEmail ?? order.ShippingEmail;
                await _emailService.SendAsync(toEmail, $"Your NOVA Order #{order.OrderNumber} is Confirmed", body);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send confirmation email for order {OrderId}", order.Id);
            }
        }

        public async Task<IEnumerable<Order>> GetByUserAsync(string userId) =>
            await _db.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task<Order?> GetByNumberAsync(string orderNumber) =>
            await _db.Orders
                .Include(o => o.Items)
                .Include(o => o.StatusHistory)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        public async Task UpdateStatusAsync(int orderId, OrderStatus status, string? note)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null) return;

            order.Status = status;
            _db.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = orderId,
                Status = status,
                Note = note
            });

            await _db.SaveChangesAsync();

            try
            {
                var toEmail = order.GuestEmail ?? order.ShippingEmail;
                await _emailService.SendAsync(toEmail, $"Order #{order.OrderNumber} Update: {status}",
                    $"Your order status has been updated to: {status}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send status update email for order {OrderId}", orderId);
            }
        }

        private int GetProductIdFromVariant(int variantId) =>
            _db.ProductVariants.Where(v => v.Id == variantId).Select(v => v.ProductId).FirstOrDefault();

        private string BuildConfirmationEmail(Order order)
        {
            var items = string.Join("\n", order.Items.Select(i =>
                $"- {i.ProductName} ({i.VariantLabel}) x{i.Quantity} — ${i.LineTotal:0.00}"));

            return $@"Hi {order.ShippingName},

Thanks for your order! Here's a summary:

Order #: {order.OrderNumber}
Payment Method: {order.PaymentMethod}

Items:
{items}

Subtotal: ${order.Subtotal:0.00}
Tax: ${order.Tax:0.00}
Discount: ${order.Discount:0.00}
Total: ${order.Total:0.00}

Shipping to:
{order.ShippingName}
{order.ShippingAddress}
{order.ShippingCity}, {order.ShippingCountry}

We'll notify you once your order ships.

— NOVA";
        }

        public async Task<List<Order>> GetAllForAdminAsync(string? statusFilter)
        {
            var query = _db.Orders
                .Include(o => o.Items)
                .AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter) &&
                Enum.TryParse<OrderStatus>(statusFilter, out var status))
            {
                query = query.Where(o => o.Status == status);
            }

            return await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id) =>
            await _db.Orders
                .Include(o => o.Items)
                .Include(o => o.StatusHistory.OrderByDescending(h => h.ChangedAt))
                .FirstOrDefaultAsync(o => o.Id == id);
    }
}