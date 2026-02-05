using System.Globalization;
using System.Text;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.OutboundOrders;

public sealed class OutboundOrderReportService : IOutboundOrderReportService
{
    private readonly IOutboundOrderRepository _orderRepository;
    private readonly IOutboundPackageRepository _packageRepository;
    private readonly IOutboundShipmentRepository _shipmentRepository;

    public OutboundOrderReportService(
        IOutboundOrderRepository orderRepository,
        IOutboundPackageRepository packageRepository,
        IOutboundShipmentRepository shipmentRepository)
    {
        _orderRepository = orderRepository;
        _packageRepository = packageRepository;
        _shipmentRepository = shipmentRepository;
    }

    public async Task<RequestResult<OutboundOrderShippingReportDto>> GetShippingReportAsync(Guid outboundOrderId, CancellationToken cancellationToken)
    {
        if (outboundOrderId == Guid.Empty)
        {
            return RequestResult<OutboundOrderShippingReportDto>.Failure("outbound_orders.report.required", "Outbound order is required.");
        }

        var order = await _orderRepository.GetByIdAsync(outboundOrderId, cancellationToken);
        if (order is null)
        {
            return RequestResult<OutboundOrderShippingReportDto>.Failure("outbound_orders.report.not_found", "Outbound order not found.");
        }

        var packages = await _packageRepository.ListByOrderIdAsync(order.Id, cancellationToken);
        var shipments = await _shipmentRepository.ListByOrderIdAsync(order.Id, cancellationToken);

        var expected = BuildExpectedMap(order);
        var shipped = BuildShippedMap(order, packages, shipments);

        var lines = BuildLines(expected, shipped);
        var pendingLines = lines.Where(x => x.Variance < 0).ToList();
        var pendingQuantity = pendingLines.Sum(x => Math.Abs(x.Variance));

        var summary = new OutboundOrderShippingReportSummaryDto(
            lines.Sum(x => x.ExpectedQuantity),
            lines.Sum(x => x.ShippedQuantity),
            lines.Sum(x => x.Variance),
            lines.Count,
            pendingLines.Count,
            pendingQuantity,
            shipments.Count,
            shipments.SelectMany(s => s.Items).Select(i => i.OutboundPackageId).Distinct().Count());

        var report = new OutboundOrderShippingReportDto(
            order.Id,
            order.OrderNumber,
            order.Warehouse?.Name ?? string.Empty,
            order.CarrierName,
            order.ExpectedShipDate,
            (int)order.Status,
            order.CreatedAtUtc,
            summary,
            lines,
            pendingLines);

        return RequestResult<OutboundOrderShippingReportDto>.Success(report);
    }

    public async Task<RequestResult<OutboundOrderShippingReportExportDto>> ExportShippingReportAsync(Guid outboundOrderId, CancellationToken cancellationToken)
    {
        var reportResult = await GetShippingReportAsync(outboundOrderId, cancellationToken);
        if (!reportResult.IsSuccess || reportResult.Value is null)
        {
            return RequestResult<OutboundOrderShippingReportExportDto>.Failure(
                reportResult.ErrorCode ?? "outbound_orders.report.failed",
                reportResult.ErrorMessage ?? "Failed to build outbound order report.");
        }

        var report = reportResult.Value;
        var csv = BuildCsv(report);
        var fileName = $"OS-{report.OrderNumber}-ShippingReport.csv";

        return RequestResult<OutboundOrderShippingReportExportDto>.Success(
            new OutboundOrderShippingReportExportDto(fileName, "text/csv", Encoding.UTF8.GetBytes(csv)));
    }

    private static Dictionary<ReportLineKey, ReportLineValue> BuildExpectedMap(OutboundOrder order)
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

    private static Dictionary<ReportLineKey, ReportLineValue> BuildShippedMap(
        OutboundOrder order,
        IReadOnlyList<OutboundPackage> packages,
        IReadOnlyList<OutboundShipment> shipments)
    {
        var shippedPackageIds = shipments
            .SelectMany(s => s.Items)
            .Select(i => i.OutboundPackageId)
            .ToHashSet();

        var orderItemsById = order.Items.ToDictionary(i => i.Id, i => i);
        var map = new Dictionary<ReportLineKey, ReportLineValue>();

        foreach (var package in packages.Where(p => shippedPackageIds.Contains(p.Id)))
        {
            foreach (var item in package.Items)
            {
                orderItemsById.TryGetValue(item.OutboundOrderItemId, out var orderItem);
                var key = new ReportLineKey(
                    item.ProductId,
                    item.UomId,
                    orderItem?.LotCode,
                    orderItem?.ExpirationDate);

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

    private static List<OutboundOrderShippingReportLineDto> BuildLines(
        Dictionary<ReportLineKey, ReportLineValue> expected,
        Dictionary<ReportLineKey, ReportLineValue> shipped)
    {
        var keys = expected.Keys.Union(shipped.Keys).ToList();
        var lines = new List<OutboundOrderShippingReportLineDto>();

        foreach (var key in keys)
        {
            expected.TryGetValue(key, out var expectedValue);
            shipped.TryGetValue(key, out var shippedValue);

            var productCode = expectedValue.ProductCode ?? shippedValue.ProductCode;
            var productName = expectedValue.ProductName ?? shippedValue.ProductName;
            var uomCode = expectedValue.UomCode ?? shippedValue.UomCode;
            var expectedQty = expectedValue.Quantity;
            var shippedQty = shippedValue.Quantity;

            lines.Add(new OutboundOrderShippingReportLineDto(
                key.ProductId,
                productCode,
                productName,
                key.UomId,
                uomCode,
                key.LotCode,
                key.ExpirationDate,
                expectedQty,
                shippedQty,
                shippedQty - expectedQty));
        }

        return lines
            .OrderBy(l => l.ProductCode)
            .ThenBy(l => l.LotCode)
            .ToList();
    }

    private static string BuildCsv(OutboundOrderShippingReportDto report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ProductCode,ProductName,Uom,LotCode,ExpirationDate,ExpectedQty,ShippedQty,Variance");

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
            sb.Append(line.ShippedQuantity.ToString(CultureInfo.InvariantCulture));
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
