using Microsoft.EntityFrameworkCore;
using NovaStore.Data;
using NovaStore.Models;
using NovaStore.Services.Interfaces;
using NovaStore.ViewModels.Shop;

namespace NovaStore.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _db;
        public ProductService(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _db.Products.Include(p => p.Category)
                               .Include(p => p.Images)
                               .Where(p => p.IsActive)
                               .ToListAsync();

        public async Task<Product?> GetBySlugAsync(string slug) =>
            await _db.Products.Include(p => p.Category)
                               .Include(p => p.Collection)
                               .Include(p => p.Images)
                               .Include(p => p.Variants)
                               .Include(p => p.Reviews)
                               .FirstOrDefaultAsync(p => p.Slug == slug);

        public async Task<IEnumerable<Product>> GetFeaturedAsync() =>
            await _db.Products.Include(p => p.Images)
                               .Where(p => p.IsActive && p.IsFeatured)
                               .ToListAsync();

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId) =>
            await _db.Products.Include(p => p.Images)
                               .Where(p => p.IsActive && p.CategoryId == categoryId)
                               .ToListAsync();

        public async Task<IEnumerable<Product>> GetByCollectionAsync(int collectionId) =>
            await _db.Products.Include(p => p.Images)
                               .Where(p => p.IsActive && p.CollectionId == collectionId)
                               .ToListAsync();

        public async Task<IEnumerable<Product>> SearchAsync(string query) =>
            await _db.Products.Include(p => p.Images)
                               .Where(p => p.IsActive &&
                                      (p.Name.Contains(query) ||
                                       (p.Description != null && p.Description.Contains(query))))
                               .ToListAsync();

        public async Task CreateAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                product.IsActive = false;
                await _db.SaveChangesAsync();
            }
        }

        private static ProductCardViewModel ToCardViewModel(Product p)
        {
            return new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                OldPrice = p.OldPrice,
                ImageFileName = p.Images.OrderBy(i => i.SortOrder)
                                        .FirstOrDefault()?.FileName,
                CategoryName = p.Category?.Name ?? "",
                IsNew = p.CreatedAt > DateTime.UtcNow.AddDays(-30)
            };
        }

        public async Task<IEnumerable<ProductCardViewModel>> GetFeaturedCardsAsync()
        {
            var products = await _db.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.IsFeatured)
                .ToListAsync();

            return products.Select(ToCardViewModel);
        }

        public async Task<ProductDetailViewModel?> GetDetailBySlugAsync(string slug)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Collection)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Include(p => p.Reviews.Where(r => r.IsApproved))
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

            if (product == null) return null;

            return new ProductDetailViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                Price = product.Price,
                OldPrice = product.OldPrice,
                CategoryName = product.Category?.Name ?? "",
                CollectionName = product.Collection?.Name,
                ImageFileNames = product.Images.OrderBy(i => i.SortOrder).Select(i => i.FileName).ToList(),
                Variants = product.Variants.Select(v => new VariantOption
                {
                    Id = v.Id,
                    Size = v.Size,
                    Color = v.Color,
                    StockQuantity = v.StockQuantity
                }).ToList(),
                Reviews = product.Reviews.Select(r => new ProductReview
                {
                    UserName = r.User.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList()
            };
        }
    }
}