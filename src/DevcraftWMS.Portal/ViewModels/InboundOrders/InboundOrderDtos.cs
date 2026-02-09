namespace DevcraftWMS.Portal.ViewModels.InboundOrders;

public sealed record InboundOrderListItemDto(
    Guid Id,
    string OrderNumber,
    string AsnNumber,
    string CustomerName,
    string WarehouseName,
    int Status,
    int Priority,
    DateOnly? ExpectedArrivalDate,
    DateTime CreatedAtUtc,
    bool IsEmergency,
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
    int Status,
    int Priority,
    int InspectionLevel,
    bool IsEmergency,
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
    int FromStatus,
    int ToStatus,
    string? Notes,
    DateTime CreatedAtUtc);

public sealed record ConvertInboundOrderRequest(Guid AsnId, string? Notes);

public sealed record UpdateInboundOrderParametersRequest(
    int InspectionLevel,
    int Priority,
    string? SuggestedDock);

public sealed record CancelInboundOrderRequest(string Reason);

public sealed record CompleteInboundOrderRequest(bool AllowPartial, string? Notes);
