using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Receipts;

public sealed record ReceiptListItemDto(
    Guid Id,
    Guid? InboundOrderId,
    string? InboundOrderNumber,
    string ReceiptNumber,
    string? DocumentNumber,
    string? SupplierName,
    Guid WarehouseId,
    string WarehouseName,
    ReceiptStatus Status,
    DateTime? StartedAtUtc,
    DateTime? ReceivedAtUtc,
    int ItemsCount,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record ReceiptDetailDto(
    Guid Id,
    Guid CustomerId,
    Guid WarehouseId,
    string WarehouseName,
    Guid? InboundOrderId,
    string? InboundOrderNumber,
    string ReceiptNumber,
    string? DocumentNumber,
    string? SupplierName,
    ReceiptStatus Status,
    DateTime? StartedAtUtc,
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
    decimal? ExpectedWeightKg,
    decimal? ExpectedVolumeCm3,
    decimal? ActualWeightKg,
    decimal? ActualVolumeCm3,
    decimal? WeightDeviationPercent,
    decimal? VolumeDeviationPercent,
    bool IsMeasurementOutOfRange,
    bool IsActive,
    DateTime CreatedAtUtc);
