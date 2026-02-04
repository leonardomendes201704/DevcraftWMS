using DevcraftWMS.Domain.Entities;
using System.Linq;

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
            task.AssignedToUserEmail,
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
            task.AssignedToUserEmail,
            task.Status,
            task.IsActive,
            task.CreatedAtUtc,
            task.AssignmentEvents
                .OrderByDescending(x => x.AssignedAtUtc)
                .Select(x => new PutawayTaskAssignmentEventDto(
                    x.Id,
                    x.FromUserId,
                    x.FromUserEmail,
                    x.ToUserId,
                    x.ToUserEmail,
                    x.Reason,
                    x.AssignedAtUtc))
                .ToList());
}
