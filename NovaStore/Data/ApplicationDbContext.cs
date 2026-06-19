using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaStore.Models;

namespace NovaStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // All your tables
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Product
            builder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price)
                      .HasColumnType("decimal(18,2)");
                entity.Property(p => p.OldPrice)
                      .HasColumnType("decimal(18,2)");
                entity.HasIndex(p => p.Slug).IsUnique();
            });

            // Category
            builder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });

            // Collection
            builder.Entity<Collection>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });

            // ProductVariant — IsAvailable is computed, not stored
            builder.Entity<ProductVariant>(entity =>
            {
                entity.Ignore(v => v.IsAvailable);
            });

            // Coupon — IsValid is computed, not stored
            builder.Entity<Coupon>(entity =>
            {
                entity.Property(c => c.DiscountPercent)
                      .HasColumnType("decimal(5,2)");
                entity.Ignore(c => c.IsValid);
                entity.HasIndex(c => c.Code).IsUnique();
            });

            // Order
            builder.Entity<Order>(entity =>
            {
                entity.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(o => o.Tax).HasColumnType("decimal(18,2)");
                entity.Property(o => o.Discount).HasColumnType("decimal(18,2)");
                entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
                entity.HasIndex(o => o.OrderNumber).IsUnique();

                // Prevent cascade delete conflicts
                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // OrderItem
            builder.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(oi => oi.LineTotal).HasColumnType("decimal(18,2)");

                entity.HasOne(oi => oi.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(oi => oi.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(oi => oi.ProductVariant)
                      .WithMany(v => v.OrderItems)
                      .HasForeignKey(oi => oi.ProductVariantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // CartItem
            builder.Entity<CartItem>(entity =>
            {
                entity.HasOne(ci => ci.User)
                      .WithMany(u => u.CartItems)
                      .HasForeignKey(ci => ci.UserId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(ci => ci.ProductVariant)
                      .WithMany(v => v.CartItems)
                      .HasForeignKey(ci => ci.ProductVariantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Review
            builder.Entity<Review>(entity =>
            {
                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reviews)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Product)
                      .WithMany(p => p.Reviews)
                      .HasForeignKey(r => r.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}