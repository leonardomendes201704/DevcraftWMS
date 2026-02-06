namespace DevcraftWMS.Api.Contracts;

public sealed record CreateOutboundOrderRequest(
    Guid WarehouseId,
    string OrderNumber,
    string? CustomerReference,
    string? CarrierName,
    DateOnly? ExpectedShipDate,
    string? Notes,
    bool IsCrossDock,
    IReadOnlyList<CreateOutboundOrderItemRequest> Items);

public sealed record CreateOutboundOrderItemRequest(
    Guid ProductId,
    Guid UomId,
    decimal Quantity,
    string? LotCode,
    DateOnly? ExpirationDate);

public sealed record ReleaseOutboundOrderRequest(
    DevcraftWMS.Domain.Enums.OutboundOrderPriority Priority,
    DevcraftWMS.Domain.Enums.OutboundOrderPickingMethod PickingMethod,
    DateTime? ShippingWindowStartUtc,
    DateTime? ShippingWindowEndUtc);

public sealed record CancelOutboundOrderRequest(string Reason);
