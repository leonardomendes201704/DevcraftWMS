using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.OutboundOrders;

public sealed record OutboundOrderListItemDto(
    Guid Id,
    string OrderNumber,
    string WarehouseName,
    OutboundOrderStatus Status,
    OutboundOrderPriority Priority,
    bool IsCrossDock,
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
    OutboundOrderStatus Status,
    OutboundOrderPriority Priority,
    OutboundOrderPickingMethod? PickingMethod,
    bool IsCrossDock,
    DateTime? ShippingWindowStartUtc,
    DateTime? ShippingWindowEndUtc,
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
