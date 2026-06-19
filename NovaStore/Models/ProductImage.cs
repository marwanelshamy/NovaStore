namespace NovaStore.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string FileName { get; set; } = "";     // GUID-based, never original filename
        public string? AltText { get; set; }
        public int SortOrder { get; set; } = 0;

        // Foreign Key
        public int ProductId { get; set; }

        // Navigation
        public Product Product { get; set; } = null!;
    }
}