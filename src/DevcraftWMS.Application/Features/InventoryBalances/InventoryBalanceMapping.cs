using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.InventoryBalances;

public static class InventoryBalanceMapping
{
    public static InventoryBalanceListItemDto MapListItem(InventoryBalance balance)
        => new(
            balance.Id,
            balance.LocationId,
            balance.ProductId,
            balance.LotId,
            balance.QuantityOnHand,
            balance.QuantityReserved,
            balance.QuantityOnHand - balance.QuantityReserved,
            balance.Status,
            balance.IsActive,
            balance.CreatedAtUtc);

    public static InventoryBalanceDto Map(InventoryBalance balance)
        => new(
            balance.Id,
            balance.LocationId,
            balance.ProductId,
            balance.LotId,
            balance.QuantityOnHand,
            balance.QuantityReserved,
            balance.QuantityOnHand - balance.QuantityReserved,
            balance.Status,
            balance.IsActive,
            balance.CreatedAtUtc);
}
