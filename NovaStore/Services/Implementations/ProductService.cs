using Microsoft.EntityFrameworkCore;
using NovaStore.Data;
using NovaStore.Models;
using NovaStore.Services.Interfaces;
using NovaStore.ViewModels.Admin;
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

        public async Task<ProductListViewModel> GetShopListAsync(int? categoryId, int? collectionId, string sortBy, int page, int pageSize)
        {
            var query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsActive);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (collectionId.HasValue)
                query = query.Where(p => p.CollectionId == collectionId.Value);

            query = sortBy switch
            {
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                "price_low" => query.OrderBy(p => p.Price),
                "price_high" => query.OrderByDescending(p => p.Price),
                _ => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _db.Categories
                .Select(c => new CategoryOption
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = c.Products.Count(p => p.IsActive)
                })
                .OrderBy(c => c.Name)
                .ToListAsync();

            var collections = await _db.Collections
                .Where(c => c.IsActive)
                .Select(c => new CollectionOption { Id = c.Id, Name = c.Name })
                .ToListAsync();

            return new ProductListViewModel
            {
                Products = products.Select(ToCardViewModel).ToList(),
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize,
                Filters = new ShopFilterViewModel
                {
                    Categories = categories,
                    Collections = collections,
                    SelectedCategoryId = categoryId,
                    SelectedCollectionId = collectionId,
                    SortBy = sortBy,
                    Page = page
                }
            };
        }

        public async Task<List<Product>> GetAllForAdminAsync() =>
                    await _db.Products
                        .Include(p => p.Category)
                        .Include(p => p.Images)
                        .OrderByDescending(p => p.CreatedAt)
                        .ToListAsync();

        public async Task<AdminProductFormViewModel> GetEditFormAsync(int? id)
        {
            var categories = await _db.Categories
                .Select(c => new CategoryOptionSimple { Id = c.Id, Name = c.Name })
                .ToListAsync();

            var collections = await _db.Collections
                .Select(c => new CollectionOptionSimple { Id = c.Id, Name = c.Name })
                .ToListAsync();

            if (id == null)
            {
                return new AdminProductFormViewModel
                {
                    CategoryOptions = categories,
                    CollectionOptions = collections
                };
            }

            var product = await _db.Products
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new InvalidOperationException("Product not found.");

            return new AdminProductFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                Price = product.Price,
                OldPrice = product.OldPrice,
                CategoryId = product.CategoryId,
                CollectionId = product.CollectionId,
                IsFeatured = product.IsFeatured,
                IsActive = product.IsActive,
                CategoryOptions = categories,
                CollectionOptions = collections,
                ExistingImages = product.Images.Select(i => new ExistingImage { Id = i.Id, FileName = i.FileName }).ToList(),
                Variants = product.Variants.Select(v => new VariantInput
                {
                    Id = v.Id,
                    Size = v.Size,
                    Color = v.Color,
                    SKU = v.SKU,
                    StockQuantity = v.StockQuantity
                }).ToList()
            };
        }

        public async Task<int> SaveFromFormAsync(AdminProductFormViewModel model, string webRootPath)
        {
            Product product;

            if (model.Id == 0)
            {
                product = new Product();
                _db.Products.Add(product);
            }
            else
            {
                product = await _db.Products
                    .Include(p => p.Variants)
                    .FirstAsync(p => p.Id == model.Id);
            }

            product.Name = model.Name;
            product.Slug = model.Slug;
            product.Description = model.Description;
            product.Price = model.Price;
            product.OldPrice = model.OldPrice;
            product.CategoryId = model.CategoryId;
            product.CollectionId = model.CollectionId;
            product.IsFeatured = model.IsFeatured;
            product.IsActive = model.IsActive;

            // Handle variants — simple approach: remove all, re-add from form
            // Handle variants — update existing ones by Id, add new ones, only delete unreferenced ones
            var submittedVariants = model.Variants.Where(v => !string.IsNullOrWhiteSpace(v.Size)).ToList();
            var submittedIds = submittedVariants.Where(v => v.Id != 0).Select(v => v.Id).ToHashSet();

            // Variants that existed before but are no longer in the submitted form
            var variantsToRemove = product.Variants.Where(v => !submittedIds.Contains(v.Id)).ToList();

            foreach (var toRemove in variantsToRemove)
            {
                var hasOrders = await _db.OrderItems.AnyAsync(oi => oi.ProductVariantId == toRemove.Id);
                if (!hasOrders)
                {
                    _db.ProductVariants.Remove(toRemove);
                }
                // If it has orders, we silently keep it in the database (but it won't show in the form anymore
                // unless we explicitly reload it — acceptable tradeoff to preserve order history integrity)
            }

            foreach (var v in submittedVariants)
            {
                if (v.Id != 0)
                {
                    // Update existing variant
                    var existing = product.Variants.FirstOrDefault(pv => pv.Id == v.Id);
                    if (existing != null)
                    {
                        existing.Size = v.Size;
                        existing.Color = v.Color;
                        existing.SKU = v.SKU;
                        existing.StockQuantity = v.StockQuantity;
                    }
                }
                else
                {
                    // New variant
                    product.Variants.Add(new ProductVariant
                    {
                        Size = v.Size,
                        Color = v.Color,
                        SKU = v.SKU,
                        StockQuantity = v.StockQuantity
                    });
                }
            }

            await _db.SaveChangesAsync(); // Save first to get product.Id if new

            // Handle new image uploads
            if (model.NewImages != null && model.NewImages.Any())
            {
                var uploadFolder = Path.Combine(webRootPath, "images", "products");
                Directory.CreateDirectory(uploadFolder);

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var maxSizeBytes = 5 * 1024 * 1024; // 5MB

                int sortOrder = product.Images.Count;

                foreach (var file in model.NewImages)
                {
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(ext))
                        continue; // skip invalid file types silently — could also collect errors to show user

                    if (file.Length > maxSizeBytes || file.Length == 0)
                        continue;

                    var guidFileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadFolder, guidFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    _db.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        FileName = guidFileName,
                        SortOrder = sortOrder++
                    });
                }

                await _db.SaveChangesAsync();
            }

            return product.Id;
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _db.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                _db.ProductImages.Remove(image);
                await _db.SaveChangesAsync();
                // Note: we're not deleting the physical file here to keep this simple;
                // an orphaned file on disk is a minor issue, not a security risk.
            }
        }

        public async Task<CollectionListViewModel> GetCollectionListAsync()
        {
            var collections = await _db.Collections
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.Year)
                .ThenByDescending(c => c.IsCurrent)
                .Select(c => new CollectionSummary
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Season = c.Season,
                    Year = c.Year,
                    Description = c.Description,
                    CoverImageFileName = c.CoverImageFileName,
                    IsCurrent = c.IsCurrent,
                    ProductCount = c.Products.Count(p => p.IsActive)
                })
                .ToListAsync();

            return new CollectionListViewModel { Collections = collections };
        }

        public async Task<CollectionDetailViewModel?> GetCollectionDetailAsync(string slug)
        {
            var collection = await _db.Collections.FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);
            if (collection == null) return null;

            var products = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.CollectionId == collection.Id && p.IsActive)
                .ToListAsync();

            return new CollectionDetailViewModel
            {
                Name = collection.Name,
                Season = collection.Season,
                Year = collection.Year,
                Description = collection.Description,
                Products = products.Select(ToCardViewModel).ToList()
            };
        }
    }
}