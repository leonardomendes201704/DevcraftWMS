using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.PickingReplenishments;

public static class PickingReplenishmentMapping
{
    public static PickingReplenishmentListItemDto MapListItem(PickingReplenishmentTask task)
        => new(
            task.Id,
            task.WarehouseId,
            task.ProductId,
            task.Product?.Code ?? string.Empty,
            task.Product?.Name ?? string.Empty,
            task.FromLocation?.Code ?? string.Empty,
            task.ToLocation?.Code ?? string.Empty,
            task.QuantityPlanned,
            task.QuantityMoved,
            task.Status,
            task.IsActive,
            task.CreatedAtUtc);
}
