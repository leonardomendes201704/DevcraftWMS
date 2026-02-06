using System.Text;
using DevcraftWMS.DemoMvc.Enums;

namespace DevcraftWMS.DemoMvc.ViewModels.DockSchedules;

public sealed record DockScheduleViewModel(
    Guid Id,
    Guid WarehouseId,
    string WarehouseName,
    string DockCode,
    DateTime SlotStartUtc,
    DateTime SlotEndUtc,
    DockScheduleStatus Status,
    Guid? OutboundOrderId,
    string? OutboundOrderNumber,
    Guid? OutboundShipmentId,
    string? Notes,
    string? RescheduleReason);

public sealed record DockScheduleListItemViewModel(
    Guid Id,
    Guid WarehouseId,
    string WarehouseName,
    string DockCode,
    DateTime SlotStartUtc,
    DateTime SlotEndUtc,
    DockScheduleStatus Status,
    bool IsActive);

public sealed record CreateDockScheduleRequestViewModel(
    Guid WarehouseId,
    string DockCode,
    DateTime SlotStartUtc,
    DateTime SlotEndUtc,
    Guid? OutboundOrderId,
    string? Notes);

public sealed record RescheduleDockScheduleRequestViewModel(
    DateTime SlotStartUtc,
    DateTime SlotEndUtc,
    string Reason);

public sealed record CancelDockScheduleRequestViewModel(string Reason);

public sealed record AssignDockScheduleRequestViewModel(
    Guid? OutboundOrderId,
    Guid? OutboundShipmentId,
    string? Notes);

public sealed record DockScheduleListQueryViewModel(
    Guid? WarehouseId,
    string? DockCode,
    DockScheduleStatus? Status,
    DateTime? FromUtc,
    DateTime? ToUtc,
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
        Append(sb, "dockCode", DockCode);
        Append(sb, "status", Status);
        Append(sb, "fromUtc", FromUtc);
        Append(sb, "toUtc", ToUtc);
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

public sealed class DockScheduleListPageViewModel
{
    public DockScheduleListQueryViewModel Query { get; set; } = new(null, null, null, null, null, null, false, 1, 20, "SlotStartUtc", "asc");
    public IReadOnlyList<DockScheduleListItemViewModel> Items { get; set; } = Array.Empty<DockScheduleListItemViewModel>();
    public DevcraftWMS.DemoMvc.ViewModels.Shared.PaginationViewModel? Pagination { get; set; }
}

public sealed class DockScheduleDetailsPageViewModel
{
    public DockScheduleViewModel Schedule { get; set; } = null!;
}
