using NovaStore.ViewModels.Cart;

namespace NovaStore.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartViewModel> GetCartAsync(string? userId, string? sessionId);
        Task AddItemAsync(int variantId, int quantity, string? userId, string? sessionId);
        Task UpdateQtyAsync(int cartItemId, int quantity, string? userId);
        Task RemoveItemAsync(int cartItemId, string? userId);
        Task ClearAsync(string? userId, string? sessionId);
        Task<CartViewModel> GetCartAsync(string? userId, string? sessionId, string? couponCode = null);
    }
}