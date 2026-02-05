namespace DevcraftWMS.Api.Contracts;

public sealed record CreateOutboundOrderRequest(
    Guid WarehouseId,
    string OrderNumber,
    string? CustomerReference,
    string? CarrierName,
    DateOnly? ExpectedShipDate,
    string? Notes,
    IReadOnlyList<CreateOutboundOrderItemRequest> Items);

public sealed record CreateOutboundOrderItemRequest(
    Guid ProductId,
    Guid UomId,
    decimal Quantity,
    string? LotCode,
    DateOnly? ExpirationDate);
