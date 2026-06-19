namespace NovaStore.ViewModels.Shop
{
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string CategoryName { get; set; } = "";
        public string? CollectionName { get; set; }

        public List<string> ImageFileNames { get; set; } = new();
        public List<VariantOption> Variants { get; set; } = new();
        public List<ProductReview> Reviews { get; set; } = new();

        public bool IsOnSale => OldPrice.HasValue && OldPrice > Price;
        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;
    }

    public class VariantOption
    {
        public int Id { get; set; }
        public string Size { get; set; } = "";
        public string Color { get; set; } = "";
        public int StockQuantity { get; set; }
        public bool IsAvailable => StockQuantity > 0;
    }

    public class ProductReview
    {
        public string UserName { get; set; } = "";
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}