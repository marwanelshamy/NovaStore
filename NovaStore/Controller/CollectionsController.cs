using Microsoft.AspNetCore.Mvc;
using NovaStore.Services.Interfaces;

namespace NovaStore.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly IProductService _productService;

        public CollectionsController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _productService.GetCollectionListAsync();
            return View(model);
        }

        public async Task<IActionResult> Details(string slug)
        {
            var model = await _productService.GetCollectionDetailAsync(slug);
            if (model == null) return NotFound();
            return View(model);
        }
    }
}