using NovaStore.ViewModels.Shop;

namespace NovaStore.ViewModels.Shop
{
    public class CollectionListViewModel
    {
        public List<CollectionSummary> Collections { get; set; } = new();
    }

    public class CollectionSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public string Season { get; set; } = "";
        public int Year { get; set; }
        public string? Description { get; set; }
        public string? CoverImageFileName { get; set; }
        public bool IsCurrent { get; set; }
        public int ProductCount { get; set; }
    }

    public class CollectionDetailViewModel
    {
        public string Name { get; set; } = "";
        public string Season { get; set; } = "";
        public int Year { get; set; }
        public string? Description { get; set; }
        public List<ProductCardViewModel> Products { get; set; } = new();
    }
}