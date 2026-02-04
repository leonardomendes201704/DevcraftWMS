using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.PutawayTasks;

public sealed record PutawayTaskQuery(
    Guid? WarehouseId = null,
    Guid? ReceiptId = null,
    Guid? UnitLoadId = null,
    PutawayTaskStatus? Status = null,
    bool? IsActive = null,
    bool IncludeInactive = false,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc");

public sealed record PutawayTaskListItemViewModel(
    Guid Id,
    Guid UnitLoadId,
    Guid ReceiptId,
    Guid WarehouseId,
    string SsccInternal,
    string ReceiptNumber,
    string WarehouseName,
    PutawayTaskStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record PutawayTaskDetailViewModel(
    Guid Id,
    Guid UnitLoadId,
    Guid ReceiptId,
    Guid WarehouseId,
    string SsccInternal,
    string? SsccExternal,
    string ReceiptNumber,
    string WarehouseName,
    PutawayTaskStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class PutawayTaskListPageViewModel
{
    public IReadOnlyList<PutawayTaskListItemViewModel> Items { get; init; } = Array.Empty<PutawayTaskListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public PutawayTaskQuery Query { get; init; } = new();
}

public sealed class PutawayTaskDetailsPageViewModel
{
    public PutawayTaskDetailViewModel Task { get; init; } = default!;
}
