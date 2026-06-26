using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NovaStore.Data;
using NovaStore.Models;
using NovaStore.ViewModels.Contact;

namespace NovaStore.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ContactController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(new ContactFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("contact")]
        public async Task<IActionResult> Submit(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            _db.ContactMessages.Add(new ContactMessage
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Subject = model.Subject,
                OrderNumber = model.OrderNumber,
                Message = model.Message
            });

            await _db.SaveChangesAsync();

            model.SubmittedSuccessfully = true;
            ModelState.Clear();
            return View("Index", new ContactFormViewModel { SubmittedSuccessfully = true });
        }
    }
}