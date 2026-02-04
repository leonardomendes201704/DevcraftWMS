using System.Globalization;
using System.Text;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.InboundOrders;

public sealed class InboundOrderReportService : IInboundOrderReportService
{
    private readonly IInboundOrderRepository _inboundOrderRepository;
    private readonly IReceiptRepository _receiptRepository;
    private readonly IReceiptDivergenceRepository _divergenceRepository;

    public InboundOrderReportService(
        IInboundOrderRepository inboundOrderRepository,
        IReceiptRepository receiptRepository,
        IReceiptDivergenceRepository divergenceRepository)
    {
        _inboundOrderRepository = inboundOrderRepository;
        _receiptRepository = receiptRepository;
        _divergenceRepository = divergenceRepository;
    }

    public async Task<RequestResult<InboundOrderReceiptReportDto>> GetReceiptReportAsync(Guid inboundOrderId, CancellationToken cancellationToken)
    {
        if (inboundOrderId == Guid.Empty)
        {
            return RequestResult<InboundOrderReceiptReportDto>.Failure("inbound_orders.report.required", "Inbound order is required.");
        }

        var order = await _inboundOrderRepository.GetByIdAsync(inboundOrderId, cancellationToken);
        if (order is null)
        {
            return RequestResult<InboundOrderReceiptReportDto>.Failure("inbound_orders.report.not_found", "Inbound order not found.");
        }

        var receipts = await _receiptRepository.ListByInboundOrderIdAsync(order.Id, cancellationToken);
        var divergences = await _divergenceRepository.ListByInboundOrderAsync(order.Id, cancellationToken);

        var expected = BuildExpectedMap(order);
        var received = BuildReceivedMap(receipts);

        var lines = BuildLines(expected, received);

        var summary = new InboundOrderReceiptReportSummaryDto(
            lines.Sum(x => x.ExpectedQuantity),
            lines.Sum(x => x.ReceivedQuantity),
            lines.Sum(x => x.Variance),
            lines.Count,
            divergences.Count);

        var divergenceDtos = divergences
            .Select(d => new InboundOrderReceiptDivergenceDto(
                d.Id,
                d.ReceiptId,
                d.InboundOrderItemId,
                d.Type,
                d.Notes,
                d.Evidence.Count,
                d.CreatedAtUtc))
            .ToList();

        var report = new InboundOrderReceiptReportDto(
            order.Id,
            order.OrderNumber,
            order.Asn?.AsnNumber ?? string.Empty,
            order.Warehouse?.Name ?? string.Empty,
            order.SupplierName,
            order.DocumentNumber,
            order.ExpectedArrivalDate,
            order.Status,
            order.CreatedAtUtc,
            summary,
            lines,
            divergenceDtos);

        return RequestResult<InboundOrderReceiptReportDto>.Success(report);
    }

    public async Task<RequestResult<InboundOrderReceiptReportExportDto>> ExportReceiptReportAsync(Guid inboundOrderId, CancellationToken cancellationToken)
    {
        var reportResult = await GetReceiptReportAsync(inboundOrderId, cancellationToken);
        if (!reportResult.IsSuccess || reportResult.Value is null)
        {
            return RequestResult<InboundOrderReceiptReportExportDto>.Failure(
                reportResult.ErrorCode ?? "inbound_orders.report.failed",
                reportResult.ErrorMessage ?? "Failed to build inbound order report.");
        }

        var report = reportResult.Value;
        var csv = BuildCsv(report);
        var fileName = $"OE-{report.OrderNumber}-ReceiptReport.csv";

        return RequestResult<InboundOrderReceiptReportExportDto>.Success(
            new InboundOrderReceiptReportExportDto(fileName, "text/csv", Encoding.UTF8.GetBytes(csv)));
    }

    private static Dictionary<ReportLineKey, ReportLineValue> BuildExpectedMap(InboundOrder order)
    {
        var map = new Dictionary<ReportLineKey, ReportLineValue>();
        foreach (var item in order.Items)
        {
            var key = new ReportLineKey(
                item.ProductId,
                item.UomId,
                item.LotCode,
                item.ExpirationDate);

            if (!map.TryGetValue(key, out var value))
            {
                value = new ReportLineValue(
                    item.Product?.Code ?? string.Empty,
                    item.Product?.Name ?? string.Empty,
                    item.Uom?.Code ?? string.Empty,
                    0m);
            }

            value = value with { Quantity = value.Quantity + item.Quantity };
            map[key] = value;
        }

        return map;
    }

    private static Dictionary<ReportLineKey, ReportLineValue> BuildReceivedMap(IReadOnlyList<Domain.Entities.Receipt> receipts)
    {
        var map = new Dictionary<ReportLineKey, ReportLineValue>();
        foreach (var receipt in receipts)
        {
            foreach (var item in receipt.Items)
            {
                var lotCode = item.Lot?.Code;
                var expirationDate = item.Lot?.ExpirationDate;
                var key = new ReportLineKey(
                    item.ProductId,
                    item.UomId,
                    lotCode,
                    expirationDate);

                if (!map.TryGetValue(key, out var value))
                {
                    value = new ReportLineValue(
                        item.Product?.Code ?? string.Empty,
                        item.Product?.Name ?? string.Empty,
                        item.Uom?.Code ?? string.Empty,
                        0m);
                }

                value = value with { Quantity = value.Quantity + item.Quantity };
                map[key] = value;
            }
        }

        return map;
    }

    private static List<InboundOrderReceiptReportLineDto> BuildLines(
        Dictionary<ReportLineKey, ReportLineValue> expected,
        Dictionary<ReportLineKey, ReportLineValue> received)
    {
        var keys = expected.Keys.Union(received.Keys).ToList();
        var lines = new List<InboundOrderReceiptReportLineDto>();

        foreach (var key in keys)
        {
            expected.TryGetValue(key, out var expectedValue);
            received.TryGetValue(key, out var receivedValue);

            var productCode = expectedValue.ProductCode ?? receivedValue.ProductCode;
            var productName = expectedValue.ProductName ?? receivedValue.ProductName;
            var uomCode = expectedValue.UomCode ?? receivedValue.UomCode;
            var expectedQty = expectedValue.Quantity;
            var receivedQty = receivedValue.Quantity;

            lines.Add(new InboundOrderReceiptReportLineDto(
                key.ProductId,
                productCode,
                productName,
                key.UomId,
                uomCode,
                key.LotCode,
                key.ExpirationDate,
                expectedQty,
                receivedQty,
                receivedQty - expectedQty));
        }

        return lines
            .OrderBy(l => l.ProductCode)
            .ThenBy(l => l.LotCode)
            .ToList();
    }

    private static string BuildCsv(InboundOrderReceiptReportDto report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ProductCode,ProductName,Uom,LotCode,ExpirationDate,ExpectedQty,ReceivedQty,Variance");

        foreach (var line in report.Lines)
        {
            sb.Append(Escape(line.ProductCode));
            sb.Append(',');
            sb.Append(Escape(line.ProductName));
            sb.Append(',');
            sb.Append(Escape(line.UomCode));
            sb.Append(',');
            sb.Append(Escape(line.LotCode));
            sb.Append(',');
            sb.Append(Escape(line.ExpirationDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            sb.Append(',');
            sb.Append(line.ExpectedQuantity.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(line.ReceivedQuantity.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(line.Variance.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }

    private readonly record struct ReportLineKey(Guid ProductId, Guid UomId, string? LotCode, DateOnly? ExpirationDate);

    private readonly record struct ReportLineValue(string ProductCode, string ProductName, string UomCode, decimal Quantity);
}
