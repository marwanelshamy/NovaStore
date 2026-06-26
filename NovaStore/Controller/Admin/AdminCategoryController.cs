using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaStore.Data;
using NovaStore.Models;
using NovaStore.ViewModels.Admin;

namespace NovaStore.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _db.Categories
                .OrderBy(c => c.SortOrder)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Slug,
                    c.SortOrder,
                    ProductCount = c.Products.Count
                })
                .ToListAsync();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View(new AdminCategoryFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.Categories.Add(new Category
            {
                Name = model.Name,
                Slug = model.Slug,
                ImageFileName = model.ImageFileName,
                SortOrder = model.SortOrder
            });
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(new AdminCategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ImageFileName = category.ImageFileName,
                SortOrder = category.SortOrder
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminCategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var category = await _db.Categories.FindAsync(model.Id);
            if (category == null) return NotFound();

            category.Name = model.Name;
            category.Slug = model.Slug;
            category.ImageFileName = model.ImageFileName;
            category.SortOrder = model.SortOrder;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var hasProducts = await _db.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                TempData["Error"] = "Cannot delete a category that still has products. Move or delete those products first.";
                return RedirectToAction("Index");
            }

            var category = await _db.Categories.FindAsync(id);
            if (category != null)
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}