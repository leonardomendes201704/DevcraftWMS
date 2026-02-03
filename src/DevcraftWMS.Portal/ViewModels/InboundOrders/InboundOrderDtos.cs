namespace DevcraftWMS.Portal.ViewModels.InboundOrders;

public sealed record InboundOrderListItemDto(
    Guid Id,
    string OrderNumber,
    string AsnNumber,
    string WarehouseName,
    int Status,
    int Priority,
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
    int Status,
    int Priority,
    int InspectionLevel,
    string? SuggestedDock,
    string? CancelReason,
    DateTime? CanceledAtUtc,
    DateTime CreatedAtUtc,
    bool IsActive,
    IReadOnlyList<InboundOrderItemDto> Items);

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

public sealed record ConvertInboundOrderRequest(Guid AsnId, string? Notes);

public sealed record UpdateInboundOrderParametersRequest(
    int InspectionLevel,
    int Priority,
    string? SuggestedDock);

public sealed record CancelInboundOrderRequest(string Reason);
