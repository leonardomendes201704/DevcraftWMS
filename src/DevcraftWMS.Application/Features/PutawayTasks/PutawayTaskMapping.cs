using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.PutawayTasks;

public static class PutawayTaskMapping
{
    public static PutawayTaskListItemDto MapListItem(PutawayTask task)
        => new(
            task.Id,
            task.UnitLoadId,
            task.ReceiptId,
            task.WarehouseId,
            task.UnitLoad?.SsccInternal ?? "-",
            task.Receipt?.ReceiptNumber ?? "-",
            task.Warehouse?.Name ?? "-",
            task.Status,
            task.IsActive,
            task.CreatedAtUtc);

    public static PutawayTaskDetailDto MapDetail(PutawayTask task)
        => new(
            task.Id,
            task.UnitLoadId,
            task.ReceiptId,
            task.WarehouseId,
            task.UnitLoad?.SsccInternal ?? "-",
            task.UnitLoad?.SsccExternal,
            task.Receipt?.ReceiptNumber ?? "-",
            task.Warehouse?.Name ?? "-",
            task.Status,
            task.IsActive,
            task.CreatedAtUtc);
}
