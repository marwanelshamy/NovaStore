namespace NovaStore.Models
{
    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public string Season { get; set; } = "";
        public int Year { get; set; }
        public string? Description { get; set; }
        public string? CoverImageFileName { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsCurrent { get; set; } = false;

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}