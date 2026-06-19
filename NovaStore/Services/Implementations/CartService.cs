using Microsoft.EntityFrameworkCore;
using NovaStore.Data;
using NovaStore.Models;
using NovaStore.Services.Interfaces;
using NovaStore.ViewModels.Cart;

namespace NovaStore.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _db;

        public CartService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CartViewModel> GetCartAsync(string? userId, string? sessionId)
        {
            var query = _db.CartItems
                .Include(c => c.ProductVariant)
                    .ThenInclude(v => v.Product)
                        .ThenInclude(p => p.Images)
                .AsQueryable();

            query = userId != null
                ? query.Where(c => c.UserId == userId)
                : query.Where(c => c.SessionId == sessionId);

            var items = await query.ToListAsync();

            return new CartViewModel
            {
                Items = items.Select(c => new CartItemViewModel
                {
                    CartItemId = c.Id,
                    ProductVariantId = c.ProductVariantId,
                    ProductName = c.ProductVariant.Product.Name,
                    VariantLabel = $"{c.ProductVariant.Color} / {c.ProductVariant.Size}",
                    ImageFileName = c.ProductVariant.Product.Images.OrderBy(i => i.SortOrder).FirstOrDefault()?.FileName,
                    UnitPrice = c.ProductVariant.Product.Price,
                    Quantity = c.Quantity,
                    AvailableStock = c.ProductVariant.StockQuantity
                }).ToList()
            };
        }

        public async Task AddItemAsync(int variantId, int quantity, string? userId, string? sessionId)
        {
            var existing = userId != null
                ? await _db.CartItems.FirstOrDefaultAsync(c => c.ProductVariantId == variantId && c.UserId == userId)
                : await _db.CartItems.FirstOrDefaultAsync(c => c.ProductVariantId == variantId && c.SessionId == sessionId);

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                _db.CartItems.Add(new CartItem
                {
                    ProductVariantId = variantId,
                    Quantity = quantity,
                    UserId = userId,
                    SessionId = userId == null ? sessionId : null
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task UpdateQtyAsync(int cartItemId, int quantity, string? userId)
        {
            var item = await _db.CartItems.FindAsync(cartItemId);
            if (item == null) return;

            item.Quantity = Math.Max(1, quantity);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(int cartItemId, string? userId)
        {
            var item = await _db.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task ClearAsync(string? userId, string? sessionId)
        {
            var query = userId != null
                ? _db.CartItems.Where(c => c.UserId == userId)
                : _db.CartItems.Where(c => c.SessionId == sessionId);

            _db.CartItems.RemoveRange(await query.ToListAsync());
            await _db.SaveChangesAsync();
        }
    }
}