using System.ComponentModel.DataAnnotations;

namespace NovaStore.ViewModels.Admin
{
    public class AdminCouponFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; } = "";

        [Required(ErrorMessage = "Discount percent is required")]
        [Range(1, 100, ErrorMessage = "Enter a percent between 1 and 100")]
        public decimal DiscountPercent { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public int? UsageLimit { get; set; }
    }
}