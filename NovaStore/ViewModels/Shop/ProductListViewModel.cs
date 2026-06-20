namespace NovaStore.ViewModels.Shop
{
    public class ProductListViewModel
    {
        public List<ProductCardViewModel> Products { get; set; } = new();
        public ShopFilterViewModel Filters { get; set; } = new();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}