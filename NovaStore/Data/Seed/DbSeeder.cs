using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaStore.Models;

namespace NovaStore.Data.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await db.Database.MigrateAsync();

            // ── 1. ROLES ────────────────────────────────────────────
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Customer"))
                await roleManager.CreateAsync(new IdentityRole("Customer"));

            // ── 2. ADMIN USER ───────────────────────────────────────
            if (await userManager.FindByEmailAsync("admin@novastore.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@novastore.com",
                    Email = "admin@novastore.com",
                    FullName = "Store Admin",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@12345");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ── 3. CATEGORIES ───────────────────────────────────────
            if (!await db.Categories.AnyAsync())
            {
                db.Categories.AddRange(
                    new Category { Name = "Women", Slug = "women", SortOrder = 1 },
                    new Category { Name = "Men", Slug = "men", SortOrder = 2 },
                    new Category { Name = "Accessories", Slug = "accessories", SortOrder = 3 }
                );
                await db.SaveChangesAsync();
            }

            // ── 4. COLLECTION ───────────────────────────────────────
            if (!await db.Collections.AnyAsync())
            {
                db.Collections.Add(new Collection
                {
                    Name = "Desert Bloom",
                    Slug = "desert-bloom-ss25",
                    Season = "SS",
                    Year = 2025,
                    Description = "Inspired by the quiet miracle of desert flowers.",
                    IsActive = true,
                    IsCurrent = true
                });
                await db.SaveChangesAsync();
            }

            // ── 5. PRODUCTS ─────────────────────────────────────────
            if (!await db.Products.AnyAsync())
            {
                var women = await db.Categories.FirstAsync(c => c.Slug == "women");
                var men = await db.Categories.FirstAsync(c => c.Slug == "men");
                var accessories = await db.Categories.FirstAsync(c => c.Slug == "accessories");
                var collection = await db.Collections.FirstAsync(c => c.Slug == "desert-bloom-ss25");

                var dress = new Product
                {
                    Name = "Sand Dune Maxi Dress",
                    Slug = "sand-dune-maxi-dress",
                    Description = "A flowing silhouette in 100% Egyptian linen, structured at the shoulder with a relaxed fall through the body.",
                    Price = 189,
                    IsFeatured = true,
                    IsActive = true,
                    CategoryId = women.Id,
                    CollectionId = collection.Id,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { FileName = "sand-dune-1.jpg", SortOrder = 1 },
                        new ProductImage { FileName = "sand-dune-2.jpg", SortOrder = 2 }
                    },
                    Variants = new List<ProductVariant>
                    {
                        new ProductVariant { Size = "S", Color = "Sand", SKU = "NV-DD-0824-SND-S", StockQuantity = 5 },
                        new ProductVariant { Size = "M", Color = "Sand", SKU = "NV-DD-0824-SND-M", StockQuantity = 8 },
                        new ProductVariant { Size = "L", Color = "Sand", SKU = "NV-DD-0824-SND-L", StockQuantity = 2 }
                    }
                };

                var shirt = new Product
                {
                    Name = "Sahara Linen Shirt",
                    Slug = "sahara-linen-shirt",
                    Description = "Breathable linen shirt with a relaxed fit, perfect for warm climates.",
                    Price = 95,
                    OldPrice = 130,
                    IsFeatured = true,
                    IsActive = true,
                    CategoryId = men.Id,
                    CollectionId = collection.Id,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { FileName = "sahara-shirt-1.jpg", SortOrder = 1 }
                    },
                    Variants = new List<ProductVariant>
                    {
                        new ProductVariant { Size = "M", Color = "Ivory", SKU = "NV-SH-0824-IVR-M", StockQuantity = 10 },
                        new ProductVariant { Size = "L", Color = "Ivory", SKU = "NV-SH-0824-IVR-L", StockQuantity = 1 }
                    }
                };

                var tote = new Product
                {
                    Name = "Cairo Leather Tote",
                    Slug = "cairo-leather-tote",
                    Description = "Handcrafted leather tote bag made by artisans in Cairo's old city.",
                    Price = 245,
                    IsFeatured = true,
                    IsActive = true,
                    CategoryId = accessories.Id,
                    Images = new List<ProductImage>
                    {
                        new ProductImage { FileName = "cairo-tote-1.jpg", SortOrder = 1 }
                    },
                    Variants = new List<ProductVariant>
                    {
                        new ProductVariant { Size = "One Size", Color = "Camel", SKU = "NV-BG-0824-CML-OS", StockQuantity = 6 }
                    }
                };

                db.Products.AddRange(dress, shirt, tote);
                await db.SaveChangesAsync();
            }
        }
    }
}