using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaStore.Data;
using NovaStore.Models;
using NovaStore.ViewModels.Admin;

namespace NovaStore.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminCollectionController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminCollectionController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var collections = await _db.Collections
                .OrderByDescending(c => c.Year)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Season,
                    c.Year,
                    c.IsActive,
                    c.IsCurrent,
                    ProductCount = c.Products.Count
                })
                .ToListAsync();

            return View(collections);
        }

        public IActionResult Create()
        {
            return View(new AdminCollectionFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCollectionFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // If this one is marked "Current", un-mark any other current collection first
            if (model.IsCurrent)
            {
                var currentOnes = await _db.Collections.Where(c => c.IsCurrent).ToListAsync();
                foreach (var c in currentOnes) c.IsCurrent = false;
            }

            _db.Collections.Add(new Collection
            {
                Name = model.Name,
                Slug = model.Slug,
                Season = model.Season,
                Year = model.Year,
                Description = model.Description,
                CoverImageFileName = model.CoverImageFileName,
                IsActive = model.IsActive,
                IsCurrent = model.IsCurrent
            });
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var collection = await _db.Collections.FindAsync(id);
            if (collection == null) return NotFound();

            return View(new AdminCollectionFormViewModel
            {
                Id = collection.Id,
                Name = collection.Name,
                Slug = collection.Slug,
                Season = collection.Season,
                Year = collection.Year,
                Description = collection.Description,
                CoverImageFileName = collection.CoverImageFileName,
                IsActive = collection.IsActive,
                IsCurrent = collection.IsCurrent
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminCollectionFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var collection = await _db.Collections.FindAsync(model.Id);
            if (collection == null) return NotFound();

            if (model.IsCurrent && !collection.IsCurrent)
            {
                var currentOnes = await _db.Collections.Where(c => c.IsCurrent && c.Id != model.Id).ToListAsync();
                foreach (var c in currentOnes) c.IsCurrent = false;
            }

            collection.Name = model.Name;
            collection.Slug = model.Slug;
            collection.Season = model.Season;
            collection.Year = model.Year;
            collection.Description = model.Description;
            collection.CoverImageFileName = model.CoverImageFileName;
            collection.IsActive = model.IsActive;
            collection.IsCurrent = model.IsCurrent;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var hasProducts = await _db.Products.AnyAsync(p => p.CollectionId == id);
            if (hasProducts)
            {
                TempData["Error"] = "Cannot delete a collection that still has products. Move or delete those products first.";
                return RedirectToAction("Index");
            }

            var collection = await _db.Collections.FindAsync(id);
            if (collection != null)
            {
                _db.Collections.Remove(collection);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}