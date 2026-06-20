using NovaStore.Models.Enums;

namespace NovaStore.ViewModels.Admin
{
    public class AdminOrderDetailViewModel
    {
        public NovaStore.Models.Order Order { get; set; } = null!;
        public List<OrderStatus> AvailableStatuses { get; set; } = Enum.GetValues<OrderStatus>().ToList();
    }
}