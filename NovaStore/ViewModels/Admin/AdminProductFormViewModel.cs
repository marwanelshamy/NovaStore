using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NovaStore.ViewModels.Admin
{
    public class AdminProductFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Slug is required")]
        public string Slug { get; set; } = "";

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 100000, ErrorMessage = "Enter a valid price")]
        public decimal Price { get; set; }

        public decimal? OldPrice { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }

        public int? CollectionId { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;

        // For dropdowns
        public List<CategoryOptionSimple> CategoryOptions { get; set; } = new();
        public List<CollectionOptionSimple> CollectionOptions { get; set; } = new();

        // Existing images (when editing)
        public List<ExistingImage> ExistingImages { get; set; } = new();

        // New uploads
        public List<IFormFile>? NewImages { get; set; }

        // Variants
        public List<VariantInput> Variants { get; set; } = new();
    }

    public class CategoryOptionSimple
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class CollectionOptionSimple
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class ExistingImage
    {
        public int Id { get; set; }
        public string FileName { get; set; } = "";
    }

    public class VariantInput
    {
        public int Id { get; set; }
        public string Size { get; set; } = "";
        public string Color { get; set; } = "";
        public string SKU { get; set; } = "";
        public int StockQuantity { get; set; }
    }
}