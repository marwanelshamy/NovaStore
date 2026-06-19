using Microsoft.AspNetCore.Mvc;
using NovaStore.Services.Interfaces;

namespace NovaStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var featured = await _productService.GetFeaturedCardsAsync();
            return View(featured);
        }
    }
}