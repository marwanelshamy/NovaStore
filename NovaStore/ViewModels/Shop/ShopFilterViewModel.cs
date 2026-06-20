namespace NovaStore.ViewModels.Shop
{
    public class ShopFilterViewModel
    {
        public List<CategoryOption> Categories { get; set; } = new();
        public List<CollectionOption> Collections { get; set; } = new();

        public int? SelectedCategoryId { get; set; }
        public int? SelectedCollectionId { get; set; }
        public string SortBy { get; set; } = "featured";
        public int Page { get; set; } = 1;
    }

    public class CategoryOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int ProductCount { get; set; }
    }

    public class CollectionOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}