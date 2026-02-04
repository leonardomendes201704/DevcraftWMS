namespace DevcraftWMS.Portal.ViewModels.InboundOrders;

public sealed class InboundOrderReportViewModel
{
    public InboundOrderReceiptReportDto? Report { get; set; }
}

public sealed record InboundOrderReceiptReportDto(
    Guid InboundOrderId,
    string OrderNumber,
    string AsnNumber,
    string WarehouseName,
    string? SupplierName,
    string? DocumentNumber,
    DateOnly? ExpectedArrivalDate,
    int Status,
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
    int Type,
    string? Notes,
    int EvidenceCount,
    DateTime CreatedAtUtc);
