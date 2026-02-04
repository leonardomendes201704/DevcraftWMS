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
            GetAvailableQuantity(balance),
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
            GetAvailableQuantity(balance),
            balance.Status,
            balance.IsActive,
            balance.CreatedAtUtc);

    private static decimal GetAvailableQuantity(InventoryBalance balance)
        => balance.Status == Domain.Enums.InventoryBalanceStatus.Blocked
            ? 0
            : balance.QuantityOnHand - balance.QuantityReserved;
}
