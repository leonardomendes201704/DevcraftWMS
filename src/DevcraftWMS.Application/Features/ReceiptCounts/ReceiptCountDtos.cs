using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.ReceiptCounts;

public sealed record ReceiptExpectedItemDto(
    Guid InboundOrderItemId,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    Guid UomId,
    string UomCode,
    decimal ExpectedQuantity,
    string? LotCode,
    DateOnly? ExpirationDate);

public sealed record ReceiptCountDto(
    Guid Id,
    Guid ReceiptId,
    Guid InboundOrderItemId,
    string ProductCode,
    string ProductName,
    string UomCode,
    decimal ExpectedQuantity,
    decimal CountedQuantity,
    decimal Variance,
    ReceiptCountMode Mode,
    bool IsDivergent,
    string? Notes,
    DateTime CreatedAtUtc);
