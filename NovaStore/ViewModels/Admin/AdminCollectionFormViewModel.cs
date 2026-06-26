using System.ComponentModel.DataAnnotations;

namespace NovaStore.ViewModels.Admin
{
    public class AdminCollectionFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Slug is required")]
        public string Slug { get; set; } = "";

        [Required(ErrorMessage = "Season is required")]
        public string Season { get; set; } = "";

        [Required(ErrorMessage = "Year is required")]
        [Range(2000, 2100, ErrorMessage = "Enter a valid year")]
        public int Year { get; set; }

        public string? Description { get; set; }

        public string? CoverImageFileName { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsCurrent { get; set; }
    }
}