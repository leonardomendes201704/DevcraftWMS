using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Returns;

public sealed record ReturnItemDto(
    Guid Id,
    Guid ProductId,
    Guid UomId,
    Guid? LotId,
    string ProductCode,
    string ProductName,
    string UomCode,
    string? LotCode,
    DateOnly? ExpirationDate,
    decimal QuantityExpected,
    decimal QuantityReceived,
    ReturnItemDisposition Disposition,
    string? DispositionNotes);

public sealed record ReturnOrderDto(
    Guid Id,
    Guid WarehouseId,
    Guid? OutboundOrderId,
    string ReturnNumber,
    string WarehouseName,
    string? OutboundOrderNumber,
    ReturnStatus Status,
    string? Notes,
    DateTime? ReceivedAtUtc,
    DateTime? CompletedAtUtc,
    IReadOnlyList<ReturnItemDto> Items);

public sealed record ReturnOrderListItemDto(
    Guid Id,
    Guid WarehouseId,
    string ReturnNumber,
    string WarehouseName,
    ReturnStatus Status,
    int ItemsCount,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record CreateReturnItemInput(
    Guid ProductId,
    Guid UomId,
    string? LotCode,
    DateOnly? ExpirationDate,
    decimal QuantityExpected);

public sealed record CompleteReturnItemInput(
    Guid ReturnItemId,
    decimal QuantityReceived,
    ReturnItemDisposition Disposition,
    string? DispositionNotes,
    Guid? LocationId);
