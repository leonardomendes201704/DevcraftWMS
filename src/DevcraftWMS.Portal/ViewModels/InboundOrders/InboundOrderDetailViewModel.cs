namespace DevcraftWMS.Portal.ViewModels.InboundOrders;

public sealed class InboundOrderDetailViewModel
{
    public InboundOrderDetailDto? Order { get; set; }
    public IReadOnlyList<InboundOrderItemDto> Items { get; set; } = Array.Empty<InboundOrderItemDto>();
    public UpdateInboundOrderParametersViewModel Parameters { get; set; } = new();
    public CancelInboundOrderViewModel Cancel { get; set; } = new();
}

public sealed class UpdateInboundOrderParametersViewModel
{
    public int InspectionLevel { get; set; }
    public int Priority { get; set; }
    public string? SuggestedDock { get; set; }
}

public sealed class CancelInboundOrderViewModel
{
    public string Reason { get; set; } = string.Empty;
}
