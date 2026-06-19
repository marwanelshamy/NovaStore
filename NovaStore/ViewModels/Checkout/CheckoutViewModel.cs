using System.ComponentModel.DataAnnotations;
using NovaStore.Models.Enums;
using NovaStore.ViewModels.Cart;

namespace NovaStore.ViewModels.Checkout
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = "";

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; } = "Egypt";

        public string? Notes { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;

        // Populated by the controller, not user input
        public CartViewModel? Cart { get; set; }
    }
}