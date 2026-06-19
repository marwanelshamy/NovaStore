using Microsoft.AspNetCore.Mvc;
using NovaStore.Services.Interfaces;

namespace NovaStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Details(string id)
        {
            var product = await _productService.GetDetailBySlugAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}