using System.ComponentModel.DataAnnotations;

namespace NovaStore.ViewModels.Admin
{
    public class AdminCategoryFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Slug is required")]
        public string Slug { get; set; } = "";

        public string? ImageFileName { get; set; }

        public int SortOrder { get; set; }
    }
}