using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevcraftWMS.DemoMvc.ViewModels.PickingTasks;

public sealed record PickingTaskQuery(
    Guid? WarehouseId = null,
    Guid? OutboundOrderId = null,
    Guid? AssignedUserId = null,
    PickingTaskStatus? Status = null,
    bool? IsActive = null,
    bool IncludeInactive = false,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc");

public sealed record PickingTaskListItemViewModel(
    Guid Id,
    Guid OutboundOrderId,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    PickingTaskStatus Status,
    int Sequence,
    Guid? AssignedUserId,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record PickingTaskItemViewModel(
    Guid Id,
    Guid OutboundOrderItemId,
    Guid ProductId,
    Guid UomId,
    Guid? LotId,
    Guid? LocationId,
    string ProductCode,
    string ProductName,
    string UomCode,
    string? LotCode,
    string? LocationCode,
    decimal QuantityPlanned,
    decimal QuantityPicked);

public sealed record PickingTaskDetailViewModel(
    Guid Id,
    Guid OutboundOrderId,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    PickingTaskStatus Status,
    int Sequence,
    Guid? AssignedUserId,
    string? Notes,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    bool IsActive,
    DateTime CreatedAtUtc,
    IReadOnlyList<PickingTaskItemViewModel> Items);

public sealed class PickingTaskListPageViewModel
{
    public IReadOnlyList<PickingTaskListItemViewModel> Items { get; init; } = Array.Empty<PickingTaskListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public PickingTaskQuery Query { get; init; } = new();
}

public sealed class PickingTaskDetailsPageViewModel
{
    public PickingTaskDetailViewModel Task { get; init; } = default!;
    public PickingTaskConfirmViewModel Confirm { get; init; } = new();
    public IReadOnlyList<SelectListItem> StatusOptions { get; init; } = Array.Empty<SelectListItem>();
}

public sealed class PickingTaskConfirmViewModel
{
    public List<PickingTaskConfirmItemViewModel> Items { get; init; } = new();
    public string? Notes { get; set; }
}

public sealed class PickingTaskConfirmItemViewModel
{
    public Guid PickingTaskItemId { get; set; }
    public decimal QuantityPicked { get; set; }
}
