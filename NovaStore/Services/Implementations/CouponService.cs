using Microsoft.EntityFrameworkCore;
using NovaStore.Data;
using NovaStore.Models;
using NovaStore.Services.Interfaces;

namespace NovaStore.Services.Implementations
{
    public class CouponService : ICouponService
    {
        private readonly ApplicationDbContext _db;
        public CouponService(ApplicationDbContext db) => _db = db;

        public async Task<Coupon?> ValidateAsync(string code)
        {
            var coupon = await _db.Coupons
                .FirstOrDefaultAsync(c => c.Code.ToUpper() == code.ToUpper());

            return coupon != null && coupon.IsValid ? coupon : null;
        }

        public async Task<decimal> ApplyAsync(string code, decimal subtotal)
        {
            var coupon = await ValidateAsync(code);
            if (coupon == null) return 0;

            return subtotal * (coupon.DiscountPercent / 100m);
        }

        public async Task<IEnumerable<Coupon>> GetAllAsync() =>
            await _db.Coupons.OrderByDescending(c => c.CreatedAt).ToListAsync();

        public async Task CreateAsync(Coupon coupon)
        {
            _db.Coupons.Add(coupon);
            await _db.SaveChangesAsync();
        }

        public async Task DisableAsync(int id)
        {
            var coupon = await _db.Coupons.FindAsync(id);
            if (coupon != null)
            {
                coupon.IsActive = false;
                await _db.SaveChangesAsync();
            }
        }
    }
}