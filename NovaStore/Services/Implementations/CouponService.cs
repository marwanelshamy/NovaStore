using NovaStore.Models;
using NovaStore.Services.Interfaces;

namespace NovaStore.Services.Implementations
{
    public class CouponService : ICouponService
    {
        public Task<Coupon?> ValidateAsync(string code) => Task.FromResult<Coupon?>(null);
        public Task<decimal> ApplyAsync(string code, decimal subtotal) => Task.FromResult(0m);
        public Task<IEnumerable<Coupon>> GetAllAsync() => Task.FromResult(Enumerable.Empty<Coupon>());
        public Task CreateAsync(Coupon coupon) => Task.CompletedTask;
        public Task DisableAsync(int id) => Task.CompletedTask;
    }
}