using Microsoft.EntityFrameworkCore;
using NovaStore.Data;
using NovaStore.Models;
using NovaStore.Services.Interfaces;

namespace NovaStore.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _db;
        public InventoryService(ApplicationDbContext db) => _db = db;

        public async Task ReduceStockAsync(int variantId, int quantity)
        {
            var variant = await _db.ProductVariants.FindAsync(variantId);
            if (variant != null)
            {
                variant.StockQuantity = Math.Max(0, variant.StockQuantity - quantity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task RestoreStockAsync(int variantId, int quantity)
        {
            var variant = await _db.ProductVariants.FindAsync(variantId);
            if (variant != null)
            {
                variant.StockQuantity += quantity;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductVariant>> GetLowStockVariantsAsync(int threshold) =>
            await _db.ProductVariants
                .Include(v => v.Product)
                .Where(v => v.StockQuantity <= threshold && v.StockQuantity > 0)
                .ToListAsync();

        public async Task<bool> IsVariantAvailableAsync(int variantId, int quantity)
        {
            var variant = await _db.ProductVariants.FindAsync(variantId);
            return variant != null && variant.StockQuantity >= quantity;
        }
    }
}