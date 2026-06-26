using System.ComponentModel.DataAnnotations;

namespace NovaStore.ViewModels.Contact
{
    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = "";

        public string? Phone { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; } = "";

        public string? OrderNumber { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters")]
        public string Message { get; set; } = "";

        public bool SubmittedSuccessfully { get; set; }
    }
}