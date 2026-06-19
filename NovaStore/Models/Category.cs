namespace NovaStore.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public string? ImageFileName { get; set; }
        public int SortOrder { get; set; } = 0;

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}