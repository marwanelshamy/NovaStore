using NovaStore.Models;

namespace NovaStore.Services.Interfaces
{
    public interface IInventoryService
    {
        Task ReduceStockAsync(int variantId, int quantity);
        Task RestoreStockAsync(int variantId, int quantity);
        Task<IEnumerable<ProductVariant>> GetLowStockVariantsAsync(int threshold);
        Task<bool> IsVariantAvailableAsync(int variantId, int quantity);
    }
}