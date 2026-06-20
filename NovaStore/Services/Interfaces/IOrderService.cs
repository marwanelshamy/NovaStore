using NovaStore.Models;
using NovaStore.Models.Enums;
using NovaStore.ViewModels.Checkout;

namespace NovaStore.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CheckoutViewModel model, string? userId, string? sessionId);
        Task ConfirmOrderAsync(int orderId);
        Task<IEnumerable<Order>> GetByUserAsync(string userId);
        Task<Order?> GetByNumberAsync(string orderNumber);
        Task UpdateStatusAsync(int orderId, OrderStatus status, string? note);

        Task<List<Order>> GetAllForAdminAsync(string? statusFilter);
        Task<Order?> GetByIdAsync(int id);
    }
}