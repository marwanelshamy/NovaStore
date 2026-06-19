using NovaStore.Models;
using NovaStore.Services.Interfaces;

namespace NovaStore.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        public Task ReduceStockAsync(int variantId, int quantity) => Task.CompletedTask;
        public Task RestoreStockAsync(int variantId, int quantity) => Task.CompletedTask;

        public Task<IEnumerable<ProductVariant>> GetLowStockVariantsAsync(int threshold) =>
            Task.FromResult(Enumerable.Empty<ProductVariant>());

        public Task<bool> IsVariantAvailableAsync(int variantId, int quantity) =>
            Task.FromResult(true);
    }
}