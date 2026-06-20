namespace NovaStore.ViewModels.Admin
{
    public class AdminOrderListViewModel
    {
        public List<NovaStore.Models.Order> Orders { get; set; } = new();
        public string? StatusFilter { get; set; }
    }
}