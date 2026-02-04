namespace DevcraftWMS.DemoMvc.ViewModels.InboundOrders;

public sealed class InboundOrderReportPageViewModel
{
    public InboundOrderReceiptReportViewModel? Report { get; set; }
}

public sealed record InboundOrderReceiptReportViewModel(
    Guid InboundOrderId,
    string OrderNumber,
    string AsnNumber,
    string WarehouseName,
    string? SupplierName,
    string? DocumentNumber,
    DateOnly? ExpectedArrivalDate,
    int Status,
    DateTime CreatedAtUtc,
    InboundOrderReceiptReportSummaryViewModel Summary,
    IReadOnlyList<InboundOrderReceiptReportLineViewModel> Lines,
    IReadOnlyList<InboundOrderReceiptReportLineViewModel> PendingLines,
    IReadOnlyList<InboundOrderReceiptDivergenceViewModel> Divergences);

public sealed record InboundOrderReceiptReportSummaryViewModel(
    decimal TotalExpected,
    decimal TotalReceived,
    decimal TotalVariance,
    int LineCount,
    int PendingLineCount,
    decimal PendingQuantity,
    int DivergenceCount);

public sealed record InboundOrderReceiptReportLineViewModel(
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

public sealed record InboundOrderReceiptDivergenceViewModel(
    Guid Id,
    Guid ReceiptId,
    Guid? InboundOrderItemId,
    int Type,
    string? Notes,
    int EvidenceCount,
    DateTime CreatedAtUtc);
