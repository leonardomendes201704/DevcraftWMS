using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.ViewModels.InboundOrders;

public sealed class InboundOrderListViewModel
{
    public InboundOrderListQuery Query { get; set; } = new();
    public IReadOnlyList<InboundOrderListItemDto> Items { get; set; } = Array.Empty<InboundOrderListItemDto>();
    public int TotalCount { get; set; }
    public IReadOnlyList<WarehouseOptionDto> Warehouses { get; set; } = Array.Empty<WarehouseOptionDto>();
}
