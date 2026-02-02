using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Receipts;

public sealed record ReceiptListItemDto(
    Guid Id,
    string ReceiptNumber,
    string? DocumentNumber,
    string? SupplierName,
    Guid WarehouseId,
    string WarehouseName,
    ReceiptStatus Status,
    DateTime? ReceivedAtUtc,
    int ItemsCount,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record ReceiptDetailDto(
    Guid Id,
    Guid CustomerId,
    Guid WarehouseId,
    string WarehouseName,
    string ReceiptNumber,
    string? DocumentNumber,
    string? SupplierName,
    ReceiptStatus Status,
    DateTime? ReceivedAtUtc,
    string? Notes,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record ReceiptItemDto(
    Guid Id,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    Guid? LotId,
    string? LotCode,
    Guid LocationId,
    string LocationCode,
    Guid UomId,
    string UomCode,
    decimal Quantity,
    decimal? UnitCost,
    bool IsActive,
    DateTime CreatedAtUtc);
