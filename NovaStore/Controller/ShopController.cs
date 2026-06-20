using Microsoft.AspNetCore.Mvc;
using NovaStore.Services.Interfaces;

namespace NovaStore.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductService _productService;

        public ShopController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(int? categoryId, int? collectionId, string sortBy = "featured", int page = 1)
        {
            var model = await _productService.GetShopListAsync(categoryId, collectionId, sortBy, page, pageSize: 9);
            return View(model);
        }
    }
}