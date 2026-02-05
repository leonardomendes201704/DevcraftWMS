namespace DevcraftWMS.Portal.ViewModels.OutboundOrders;

public sealed record OutboundOrderListItemDto(
    Guid Id,
    string OrderNumber,
    string WarehouseName,
    int Status,
    int Priority,
    DateOnly? ExpectedShipDate,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record OutboundOrderDetailDto(
    Guid Id,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    string? CustomerReference,
    string? CarrierName,
    DateOnly? ExpectedShipDate,
    string? Notes,
    int Status,
    int Priority,
    string? CancelReason,
    DateTime? CanceledAtUtc,
    DateTime CreatedAtUtc,
    bool IsActive,
    IReadOnlyList<OutboundOrderItemDto> Items);

public sealed record OutboundOrderItemDto(
    Guid Id,
    Guid ProductId,
    Guid UomId,
    string ProductCode,
    string ProductName,
    string UomCode,
    decimal Quantity,
    string? LotCode,
    DateOnly? ExpirationDate);

public sealed record OutboundOrderNotificationDto(
    Guid Id,
    Guid OutboundOrderId,
    string EventType,
    int Channel,
    int Status,
    string? ToAddress,
    string? Subject,
    DateTime? SentAtUtc,
    int Attempts,
    string? LastError,
    DateTime CreatedAtUtc);
