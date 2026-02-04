using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InboundOrders;

public sealed record InboundOrderReceiptReportDto(
    Guid InboundOrderId,
    string OrderNumber,
    string AsnNumber,
    string WarehouseName,
    string? SupplierName,
    string? DocumentNumber,
    DateOnly? ExpectedArrivalDate,
    InboundOrderStatus Status,
    DateTime CreatedAtUtc,
    InboundOrderReceiptReportSummaryDto Summary,
    IReadOnlyList<InboundOrderReceiptReportLineDto> Lines,
    IReadOnlyList<InboundOrderReceiptReportLineDto> PendingLines,
    IReadOnlyList<InboundOrderReceiptDivergenceDto> Divergences);

public sealed record InboundOrderReceiptReportSummaryDto(
    decimal TotalExpected,
    decimal TotalReceived,
    decimal TotalVariance,
    int LineCount,
    int PendingLineCount,
    decimal PendingQuantity,
    int DivergenceCount);

public sealed record InboundOrderReceiptReportLineDto(
    Guid ProductId,
    string ProductCode,
    string ProductName,
    Guid UomId,
    string UomCode,
    string? LotCode,
    DateOnly? ExpirationDate,
    decimal ExpectedQuantity,
    decimal ReceivedQuantity,
    decimal Variance);

public sealed record InboundOrderReceiptDivergenceDto(
    Guid Id,
    Guid ReceiptId,
    Guid? InboundOrderItemId,
    ReceiptDivergenceType Type,
    string? Notes,
    int EvidenceCount,
    DateTime CreatedAtUtc);

public sealed record InboundOrderReceiptReportExportDto(
    string FileName,
    string ContentType,
    byte[] Content);
