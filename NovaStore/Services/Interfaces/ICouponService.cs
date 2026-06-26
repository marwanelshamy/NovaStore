using NovaStore.Models;

namespace NovaStore.Services.Interfaces
{
    public interface ICouponService
    {

        Task<Coupon?> ValidateAsync(string code);
        Task<decimal> ApplyAsync(string code, decimal subtotal);
        Task<IEnumerable<Coupon>> GetAllAsync();
        Task CreateAsync(Coupon coupon);
        Task DisableAsync(int id);
    }
}