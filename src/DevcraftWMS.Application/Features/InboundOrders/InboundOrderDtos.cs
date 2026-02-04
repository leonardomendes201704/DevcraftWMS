using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InboundOrders;

public sealed record InboundOrderListItemDto(
    Guid Id,
    string OrderNumber,
    string AsnNumber,
    string WarehouseName,
    InboundOrderStatus Status,
    InboundOrderPriority Priority,
    DateOnly? ExpectedArrivalDate,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record InboundOrderDetailDto(
    Guid Id,
    Guid AsnId,
    Guid WarehouseId,
    string OrderNumber,
    string AsnNumber,
    string WarehouseName,
    string? SupplierName,
    string? DocumentNumber,
    DateOnly? ExpectedArrivalDate,
    string? Notes,
    InboundOrderStatus Status,
    InboundOrderPriority Priority,
    InboundOrderInspectionLevel InspectionLevel,
    string? SuggestedDock,
    string? CancelReason,
    DateTime? CanceledAtUtc,
    DateTime CreatedAtUtc,
    bool IsActive,
    IReadOnlyList<InboundOrderItemDto> Items,
    IReadOnlyList<InboundOrderStatusEventDto> StatusEvents);

public sealed record InboundOrderItemDto(
    Guid Id,
    Guid ProductId,
    Guid UomId,
    string ProductCode,
    string ProductName,
    string UomCode,
    decimal Quantity,
    string? LotCode,
    DateOnly? ExpirationDate);

public sealed record InboundOrderStatusEventDto(
    Guid Id,
    InboundOrderStatus FromStatus,
    InboundOrderStatus ToStatus,
    string? Notes,
    DateTime CreatedAtUtc);
