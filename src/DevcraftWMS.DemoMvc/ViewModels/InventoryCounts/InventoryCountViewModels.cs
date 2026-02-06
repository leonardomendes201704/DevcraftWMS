using System.Text;
using DevcraftWMS.DemoMvc.Enums;

namespace DevcraftWMS.DemoMvc.ViewModels.InventoryCounts;

public sealed record InventoryCountItemViewModel(
    Guid Id,
    Guid LocationId,
    Guid ProductId,
    Guid UomId,
    Guid? LotId,
    string LocationCode,
    string ProductCode,
    string ProductName,
    string UomCode,
    string? LotCode,
    decimal QuantityExpected,
    decimal QuantityCounted);

public sealed record InventoryCountViewModel(
    Guid Id,
    Guid WarehouseId,
    Guid LocationId,
    Guid? ZoneId,
    string WarehouseName,
    string LocationCode,
    InventoryCountStatus Status,
    string? Notes,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    IReadOnlyList<InventoryCountItemViewModel> Items);

public sealed record InventoryCountListItemViewModel(
    Guid Id,
    Guid WarehouseId,
    Guid LocationId,
    string WarehouseName,
    string LocationCode,
    InventoryCountStatus Status,
    int ItemsCount,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record CreateInventoryCountRequestViewModel(
    Guid WarehouseId,
    Guid LocationId,
    Guid? ZoneId,
    string? Notes);

public sealed record CompleteInventoryCountItemRequestViewModel(
    Guid InventoryCountItemId,
    decimal QuantityCounted);

public sealed record CompleteInventoryCountRequestViewModel(
    IReadOnlyList<CompleteInventoryCountItemRequestViewModel> Items,
    string? Notes);

public sealed record InventoryCountListQueryViewModel(
    Guid? WarehouseId,
    Guid? LocationId,
    InventoryCountStatus? Status,
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
        Append(sb, "locationId", LocationId);
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

public sealed class InventoryCountListPageViewModel
{
    public InventoryCountListQueryViewModel Query { get; set; } = new(null, null, null, null, false, 1, 20, "CreatedAtUtc", "desc");
    public IReadOnlyList<InventoryCountListItemViewModel> Items { get; set; } = Array.Empty<InventoryCountListItemViewModel>();
    public DevcraftWMS.DemoMvc.ViewModels.Shared.PaginationViewModel? Pagination { get; set; }
    public string? DebugCustomerId { get; set; }
    public int? ApiStatusCode { get; set; }
    public string? ApiError { get; set; }
}

public sealed class InventoryCountDetailsPageViewModel
{
    public InventoryCountViewModel Count { get; set; } = null!;
}
