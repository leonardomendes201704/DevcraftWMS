namespace DevcraftWMS.Portal.ViewModels.OutboundOrders;

public sealed class OutboundOrderDetailViewModel
{
    public OutboundOrderDetailDto Order { get; set; } = null!;
    public IReadOnlyList<OutboundOrderItemDto> Items { get; set; } = Array.Empty<OutboundOrderItemDto>();
}

