using System.Text;
using DevcraftWMS.DemoMvc.Enums;

namespace DevcraftWMS.DemoMvc.ViewModels.Returns;

public sealed record ReturnOrderListItemViewModel(
    Guid Id,
    Guid WarehouseId,
    string ReturnNumber,
    string WarehouseName,
    ReturnStatus Status,
    int ItemsCount,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record ReturnItemViewModel(
    Guid Id,
    Guid ProductId,
    Guid UomId,
    Guid? LotId,
    string ProductCode,
    string ProductName,
    string UomCode,
    string? LotCode,
    DateOnly? ExpirationDate,
    decimal QuantityExpected,
    decimal QuantityReceived,
    ReturnItemDisposition Disposition,
    string? DispositionNotes);

public sealed record ReturnOrderViewModel(
    Guid Id,
    Guid WarehouseId,
    Guid? OutboundOrderId,
    string ReturnNumber,
    string WarehouseName,
    string? OutboundOrderNumber,
    ReturnStatus Status,
    string? Notes,
    DateTime? ReceivedAtUtc,
    DateTime? CompletedAtUtc,
    IReadOnlyList<ReturnItemViewModel> Items);

public sealed record CreateReturnItemRequestViewModel(
    Guid ProductId,
    Guid UomId,
    string? LotCode,
    DateOnly? ExpirationDate,
    decimal QuantityExpected);

public sealed record CreateReturnOrderRequestViewModel(
    Guid WarehouseId,
    string ReturnNumber,
    Guid? OutboundOrderId,
    string? Notes,
    IReadOnlyList<CreateReturnItemRequestViewModel> Items);

public sealed record CompleteReturnItemRequestViewModel(
    Guid ReturnItemId,
    decimal QuantityReceived,
    ReturnItemDisposition Disposition,
    string? DispositionNotes,
    Guid? LocationId);

public sealed record CompleteReturnOrderRequestViewModel(
    IReadOnlyList<CompleteReturnItemRequestViewModel> Items,
    string? Notes);

public sealed record ReturnListQueryViewModel(
    Guid? WarehouseId,
    string? ReturnNumber,
    ReturnStatus? Status,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir)
{
    public ReturnListQueryViewModel Normalize()
    {
        var pageNumber = PageNumber <= 0 ? 1 : PageNumber;
        var pageSize = PageSize <= 0 ? 20 : PageSize;
        if (pageSize > 200)
        {
            pageSize = 200;
        }

        var orderBy = string.IsNullOrWhiteSpace(OrderBy) ? "CreatedAtUtc" : OrderBy;
        var orderDir = string.IsNullOrWhiteSpace(OrderDir) ? "desc" : OrderDir;

        return this with
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrderBy = orderBy,
            OrderDir = orderDir
        };
    }

    public string ToQueryString()
    {
        var normalized = Normalize();
        var sb = new StringBuilder();
        Append(sb, "warehouseId", normalized.WarehouseId);
        Append(sb, "returnNumber", normalized.ReturnNumber);
        Append(sb, "status", normalized.Status);
        Append(sb, "isActive", normalized.IsActive);
        Append(sb, "includeInactive", normalized.IncludeInactive);
        Append(sb, "pageNumber", normalized.PageNumber);
        Append(sb, "pageSize", normalized.PageSize);
        Append(sb, "orderBy", normalized.OrderBy);
        Append(sb, "orderDir", normalized.OrderDir);
        return sb.Length == 0 ? string.Empty : $"?{sb}";
    }

    private static void Append(StringBuilder sb, string key, object? value)
    {
        if (value is null)
        {
            return;
        }

        if (sb.Length > 0)
        {
            sb.Append('&');
        }

        sb.Append(Uri.EscapeDataString(key));
        sb.Append('=');
        sb.Append(Uri.EscapeDataString(value.ToString() ?? string.Empty));
    }
}
