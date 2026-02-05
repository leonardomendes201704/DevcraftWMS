using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.ViewModels.OutboundOrders;

public sealed class OutboundOrderListViewModel
{
    public OutboundOrderListQuery Query { get; set; } = new();
    public IReadOnlyList<OutboundOrderListItemDto> Items { get; set; } = Array.Empty<OutboundOrderListItemDto>();
    public int TotalCount { get; set; }
    public IReadOnlyList<WarehouseOptionDto> Warehouses { get; set; } = Array.Empty<WarehouseOptionDto>();
}

