namespace NovaStore.ViewModels.Shop
{
    public class ProductCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? ImageFileName { get; set; }
        public string CategoryName { get; set; } = "";
        public bool IsNew { get; set; }
        public bool IsOnSale => OldPrice.HasValue && OldPrice > Price;
    }
}