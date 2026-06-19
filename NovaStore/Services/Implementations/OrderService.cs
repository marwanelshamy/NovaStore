using NovaStore.Models;
using NovaStore.Models.Enums;
using NovaStore.Services.Interfaces;
using NovaStore.ViewModels.Checkout;

namespace NovaStore.Services.Implementations
{
    public class OrderService : IOrderService
    {
        public Task<Order> CreateOrderAsync(CheckoutViewModel model, string? userId, string? sessionId) =>
            Task.FromResult(new Order());

        public Task ConfirmOrderAsync(int orderId) => Task.CompletedTask;

        public Task<IEnumerable<Order>> GetByUserAsync(string userId) =>
            Task.FromResult(Enumerable.Empty<Order>());

        public Task<Order?> GetByNumberAsync(string orderNumber) =>
            Task.FromResult<Order?>(null);

        public Task UpdateStatusAsync(int orderId, OrderStatus status, string? note) =>
            Task.CompletedTask;
    }
}