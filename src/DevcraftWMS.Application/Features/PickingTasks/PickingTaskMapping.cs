using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.PickingTasks;

public static class PickingTaskMapping
{
    public static PickingTaskListItemDto MapListItem(PickingTask task)
        => new(
            task.Id,
            task.OutboundOrderId,
            task.WarehouseId,
            task.OutboundOrder?.OrderNumber ?? "-",
            task.Warehouse?.Name ?? "-",
            task.Status,
            task.Sequence,
            task.AssignedUserId,
            task.IsActive,
            task.CreatedAtUtc);

    public static PickingTaskDetailDto MapDetail(PickingTask task)
        => new(
            task.Id,
            task.OutboundOrderId,
            task.WarehouseId,
            task.OutboundOrder?.OrderNumber ?? "-",
            task.Warehouse?.Name ?? "-",
            task.Status,
            task.Sequence,
            task.AssignedUserId,
            task.Notes,
            task.StartedAtUtc,
            task.CompletedAtUtc,
            task.IsActive,
            task.CreatedAtUtc,
            task.Items.Select(MapItem).ToList());

    public static PickingTaskItemDto MapItem(PickingTaskItem item)
        => new(
            item.Id,
            item.OutboundOrderItemId,
            item.ProductId,
            item.UomId,
            item.LotId,
            item.LocationId,
            item.Product?.Code ?? "-",
            item.Product?.Name ?? "-",
            item.Uom?.Code ?? "-",
            item.Lot?.Code,
            item.Location?.Code,
            item.QuantityPlanned,
            item.QuantityPicked);
}
