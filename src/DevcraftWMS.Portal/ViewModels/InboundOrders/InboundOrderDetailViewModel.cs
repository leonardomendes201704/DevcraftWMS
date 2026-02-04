namespace DevcraftWMS.Portal.ViewModels.InboundOrders;

public sealed class InboundOrderDetailViewModel
{
    public InboundOrderDetailDto? Order { get; set; }
    public IReadOnlyList<InboundOrderItemDto> Items { get; set; } = Array.Empty<InboundOrderItemDto>();
    public IReadOnlyList<InboundOrderNotificationDto> Notifications { get; set; } = Array.Empty<InboundOrderNotificationDto>();
    public UpdateInboundOrderParametersViewModel Parameters { get; set; } = new();
    public CancelInboundOrderViewModel Cancel { get; set; } = new();
    public CompleteInboundOrderViewModel Complete { get; set; } = new();
}

public sealed record InboundOrderNotificationDto(
    Guid Id,
    Guid InboundOrderId,
    string EventType,
    int Channel,
    int Status,
    string? ToAddress,
    string? Subject,
    DateTime? SentAtUtc,
    int Attempts,
    string? LastError,
    DateTime CreatedAtUtc);

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

public sealed class CompleteInboundOrderViewModel
{
    public bool AllowPartial { get; set; }
    public string? Notes { get; set; }
}
