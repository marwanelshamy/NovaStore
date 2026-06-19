using NovaStore.Models;
using NovaStore.ViewModels.Shop;
namespace NovaStore.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetBySlugAsync(string slug);
        Task<IEnumerable<Product>> GetFeaturedAsync();
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetByCollectionAsync(int collectionId);
        Task<IEnumerable<Product>> SearchAsync(string query);
        Task<IEnumerable<ProductCardViewModel>> GetFeaturedCardsAsync();
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);

        Task<ProductDetailViewModel?> GetDetailBySlugAsync(string slug);
    }
}