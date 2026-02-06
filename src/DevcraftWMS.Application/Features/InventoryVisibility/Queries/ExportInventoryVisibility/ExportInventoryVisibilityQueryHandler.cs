using System.Globalization;
using System.Text;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryVisibility.Queries.ExportInventoryVisibility;

public sealed class ExportInventoryVisibilityQueryHandler
    : IRequestHandler<ExportInventoryVisibilityQuery, RequestResult<InventoryVisibilityExportDto>>
{
    private const int ExportPageSize = 10000;
    private readonly IInventoryVisibilityService _service;
    private readonly ICurrentUserService _currentUser;

    public ExportInventoryVisibilityQueryHandler(IInventoryVisibilityService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    public async Task<RequestResult<InventoryVisibilityExportDto>> Handle(
        ExportInventoryVisibilityQuery request,
        CancellationToken cancellationToken)
    {
        var visibility = await _service.GetAsync(
            request.CustomerId,
            request.WarehouseId,
            request.ProductId,
            request.Sku,
            request.LotCode,
            request.ExpirationFrom,
            request.ExpirationTo,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            1,
            ExportPageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);

        if (!visibility.IsSuccess || visibility.Value is null)
        {
            return RequestResult<InventoryVisibilityExportDto>.Failure(
                visibility.ErrorCode ?? "inventory_visibility.export.failed",
                visibility.ErrorMessage ?? "Failed to export inventory visibility.");
        }

        var now = DateTime.UtcNow;
        var fileSuffix = now.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture);
        var format = request.Format.ToLowerInvariant();

        return format switch
        {
            "xlsx" => RequestResult<InventoryVisibilityExportDto>.Success(
                new InventoryVisibilityExportDto(
                    $"inventory-visibility-{fileSuffix}.csv",
                    "text/csv",
                    Encoding.UTF8.GetBytes(BuildCsv(visibility.Value)))),
            "pdf" => RequestResult<InventoryVisibilityExportDto>.Success(
                new InventoryVisibilityExportDto(
                    $"inventory-visibility-{fileSuffix}.pdf",
                    "text/html",
                    Encoding.UTF8.GetBytes(BuildHtml(visibility.Value, request, now)))),
            _ => RequestResult<InventoryVisibilityExportDto>.Success(
                new InventoryVisibilityExportDto(
                    $"inventory-visibility-{fileSuffix}.html",
                    "text/html",
                    Encoding.UTF8.GetBytes(BuildHtml(visibility.Value, request, now))))
        };
    }

    private string BuildCsv(InventoryVisibilityResultDto data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Section,ProductCode,ProductName,Uom,Location,Lot,ExpirationDate,OnHand,Reserved,Blocked,InProcess,Available,Status,Active,BlockedReasons,Alerts");

        foreach (var item in data.Summary.Items)
        {
            sb.Append("Summary,");
            sb.Append(Escape(item.ProductCode));
            sb.Append(',');
            sb.Append(Escape(item.ProductName));
            sb.Append(',');
            sb.Append(string.Empty);
            sb.Append(',');
            sb.Append(',');
            sb.Append(',');
            sb.Append(',');
            sb.Append(item.QuantityOnHand.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(item.QuantityReserved.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(item.QuantityBlocked.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(item.QuantityInProcess.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(item.QuantityAvailable.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(',');
            sb.Append(',');
            sb.Append(',');
            sb.Append(Escape(string.Join(" | ", item.Alerts.Select(a => a.Message))));
            sb.AppendLine();
        }

        foreach (var item in data.Locations.Items)
        {
            sb.Append("Location,");
            sb.Append(Escape(item.ProductCode));
            sb.Append(',');
            sb.Append(Escape(item.ProductName));
            sb.Append(',');
            sb.Append(string.Empty);
            sb.Append(',');
            sb.Append(Escape(item.LocationCode));
            sb.Append(',');
            sb.Append(Escape(item.LotCode));
            sb.Append(',');
            sb.Append(Escape(item.ExpirationDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            sb.Append(',');
            sb.Append(item.QuantityOnHand.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(item.QuantityReserved.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(item.QuantityBlocked.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(item.QuantityInProcess.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(item.QuantityAvailable.ToString(CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(item.Status);
            sb.Append(',');
            sb.Append(item.IsActive ? "Yes" : "No");
            sb.Append(',');
            sb.Append(Escape(string.Join(" | ", item.BlockedReasons)));
            sb.Append(',');
            sb.Append(Escape(string.Join(" | ", item.Alerts.Select(a => a.Message))));
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private string BuildHtml(InventoryVisibilityResultDto data, ExportInventoryVisibilityQuery request, DateTime generatedAtUtc)
    {
        var userLabel = _currentUser.Email ?? _currentUser.UserId?.ToString() ?? "N/A";
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html><head><meta charset=\"utf-8\" />");
        sb.AppendLine("<title>Inventory Visibility Export</title>");
        sb.AppendLine("<style>body{font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#222;}table{width:100%;border-collapse:collapse;margin:12px 0;}th,td{border:1px solid #ddd;padding:6px;text-align:left;}th{background:#f5f5f5;}h1{font-size:18px;margin-bottom:4px;}small{color:#666;}</style>");
        sb.AppendLine("</head><body>");
        sb.AppendLine($"<h1>Inventory Visibility</h1>");
        sb.AppendLine($"<small>Generated at UTC {generatedAtUtc:yyyy-MM-dd HH:mm} | User: {Escape(userLabel)} | Customer: {request.CustomerId} | Warehouse: {request.WarehouseId}</small>");
        sb.AppendLine("<h2>Summary</h2>");
        sb.AppendLine("<table><thead><tr><th>Product</th><th>UoM</th><th>On hand</th><th>Reserved</th><th>Blocked</th><th>In process</th><th>Available</th><th>Alerts</th></tr></thead><tbody>");
        foreach (var item in data.Summary.Items)
        {
            sb.Append("<tr>");
            sb.Append($"<td>{Escape(item.ProductCode)} - {Escape(item.ProductName)}</td>");
            sb.Append($"<td>{Escape(item.UomCode)}</td>");
            sb.Append($"<td>{item.QuantityOnHand:N2}</td>");
            sb.Append($"<td>{item.QuantityReserved:N2}</td>");
            sb.Append($"<td>{item.QuantityBlocked:N2}</td>");
            sb.Append($"<td>{item.QuantityInProcess:N2}</td>");
            sb.Append($"<td><strong>{item.QuantityAvailable:N2}</strong></td>");
            sb.Append($"<td>{Escape(string.Join(", ", item.Alerts.Select(a => a.Message)))}</td>");
            sb.Append("</tr>");
        }
        sb.AppendLine("</tbody></table>");

        sb.AppendLine("<h2>Locations</h2>");
        sb.AppendLine("<table><thead><tr><th>Location</th><th>Product</th><th>Lot</th><th>Expiration</th><th>On hand</th><th>Reserved</th><th>Blocked</th><th>In process</th><th>Available</th><th>Status</th><th>Active</th><th>Blocked reasons</th><th>Alerts</th></tr></thead><tbody>");
        foreach (var item in data.Locations.Items)
        {
            sb.Append("<tr>");
            sb.Append($"<td>{Escape(item.LocationCode)}</td>");
            sb.Append($"<td>{Escape(item.ProductCode)} - {Escape(item.ProductName)}</td>");
            sb.Append($"<td>{Escape(item.LotCode)}</td>");
            sb.Append($"<td>{item.ExpirationDate:yyyy-MM-dd}</td>");
            sb.Append($"<td>{item.QuantityOnHand:N2}</td>");
            sb.Append($"<td>{item.QuantityReserved:N2}</td>");
            sb.Append($"<td>{item.QuantityBlocked:N2}</td>");
            sb.Append($"<td>{item.QuantityInProcess:N2}</td>");
            sb.Append($"<td><strong>{item.QuantityAvailable:N2}</strong></td>");
            sb.Append($"<td>{item.Status}</td>");
            sb.Append($"<td>{(item.IsActive ? "Yes" : "No")}</td>");
            sb.Append($"<td>{Escape(string.Join(", ", item.BlockedReasons))}</td>");
            sb.Append($"<td>{Escape(string.Join(", ", item.Alerts.Select(a => a.Message)))}</td>");
            sb.Append("</tr>");
        }
        sb.AppendLine("</tbody></table>");
        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
    }
}
