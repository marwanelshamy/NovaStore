using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaStore.Services.Interfaces;
using NovaStore.ViewModels.Admin;

namespace NovaStore.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _env;

        public AdminProductController(IProductService productService, IWebHostEnvironment env)
        {
            _productService = productService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllForAdminAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _productService.GetEditFormAsync(null);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var reloaded = await _productService.GetEditFormAsync(null);
                model.CategoryOptions = reloaded.CategoryOptions;
                model.CollectionOptions = reloaded.CollectionOptions;
                return View(model);
            }

            await _productService.SaveFromFormAsync(model, _env.WebRootPath);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _productService.GetEditFormAsync(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var reloaded = await _productService.GetEditFormAsync(model.Id);
                model.CategoryOptions = reloaded.CategoryOptions;
                model.CollectionOptions = reloaded.CollectionOptions;
                model.ExistingImages = reloaded.ExistingImages;
                return View(model);
            }

            await _productService.SaveFromFormAsync(model, _env.WebRootPath);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id); // soft-delete (IsActive = false), already built in Step 18/22
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            await _productService.DeleteImageAsync(imageId);
            return Ok();
        }
    }
}