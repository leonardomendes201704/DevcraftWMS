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
    public string ToQueryString()
    {
        var sb = new StringBuilder();
        Append(sb, "warehouseId", WarehouseId);
        Append(sb, "returnNumber", ReturnNumber);
        Append(sb, "status", Status);
        Append(sb, "isActive", IsActive);
        Append(sb, "includeInactive", IncludeInactive);
        Append(sb, "pageNumber", PageNumber);
        Append(sb, "pageSize", PageSize);
        Append(sb, "orderBy", OrderBy);
        Append(sb, "orderDir", OrderDir);
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
