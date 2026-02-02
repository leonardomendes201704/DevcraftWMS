using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.InventoryMovements;

public static class InventoryMovementMapping
{
    public static InventoryMovementListItemDto MapListItem(InventoryMovement movement)
        => new(
            movement.Id,
            movement.FromLocationId,
            movement.FromLocation?.Code ?? string.Empty,
            movement.ToLocationId,
            movement.ToLocation?.Code ?? string.Empty,
            movement.ProductId,
            movement.Product?.Code ?? string.Empty,
            movement.Product?.Name ?? string.Empty,
            movement.LotId,
            movement.Lot?.Code,
            movement.Quantity,
            movement.Status,
            movement.PerformedAtUtc,
            movement.IsActive,
            movement.CreatedAtUtc);

    public static InventoryMovementDto MapDetails(InventoryMovement movement)
        => new(
            movement.Id,
            movement.CustomerId,
            movement.FromLocationId,
            movement.FromLocation?.Code ?? string.Empty,
            movement.ToLocationId,
            movement.ToLocation?.Code ?? string.Empty,
            movement.ProductId,
            movement.Product?.Code ?? string.Empty,
            movement.Product?.Name ?? string.Empty,
            movement.LotId,
            movement.Lot?.Code,
            movement.Quantity,
            movement.Reason,
            movement.Reference,
            movement.Status,
            movement.PerformedAtUtc,
            movement.IsActive,
            movement.CreatedAtUtc,
            movement.UpdatedAtUtc);
}
