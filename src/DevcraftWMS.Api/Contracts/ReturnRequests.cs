using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Contracts;

public sealed record CreateReturnItemRequest(
    Guid ProductId,
    Guid UomId,
    string? LotCode,
    DateOnly? ExpirationDate,
    decimal QuantityExpected);

public sealed record CreateReturnOrderRequest(
    Guid WarehouseId,
    string ReturnNumber,
    Guid? OutboundOrderId,
    string? Notes,
    IReadOnlyList<CreateReturnItemRequest> Items);

public sealed record CompleteReturnItemRequest(
    Guid ReturnItemId,
    decimal QuantityReceived,
    ReturnItemDisposition Disposition,
    string? DispositionNotes,
    Guid? LocationId);

public sealed record CompleteReturnOrderRequest(
    IReadOnlyList<CompleteReturnItemRequest> Items,
    string? Notes);
