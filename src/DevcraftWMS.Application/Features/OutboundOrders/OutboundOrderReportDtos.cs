namespace DevcraftWMS.Application.Features.OutboundOrders;

public sealed record OutboundOrderShippingReportDto(
    Guid OutboundOrderId,
    string OrderNumber,
    string WarehouseName,
    string? CarrierName,
    DateOnly? ExpectedShipDate,
    int Status,
    DateTime CreatedAtUtc,
    OutboundOrderShippingReportSummaryDto Summary,
    IReadOnlyList<OutboundOrderShippingReportLineDto> Lines,
    IReadOnlyList<OutboundOrderShippingReportLineDto> PendingLines);

public sealed record OutboundOrderShippingReportSummaryDto(
    decimal TotalExpected,
    decimal TotalShipped,
    decimal TotalVariance,
    int LineCount,
    int PendingLineCount,
    decimal PendingQuantity,
    int ShipmentCount,
    int ShippedPackageCount);

public sealed record OutboundOrderShippingReportLineDto(
    Guid ProductId,
    string ProductCode,
    string ProductName,
    Guid UomId,
    string UomCode,
    string? LotCode,
    DateOnly? ExpirationDate,
    decimal ExpectedQuantity,
    decimal ShippedQuantity,
    decimal Variance);

public sealed record OutboundOrderShippingReportExportDto(
    string FileName,
    string ContentType,
    byte[] Content);
