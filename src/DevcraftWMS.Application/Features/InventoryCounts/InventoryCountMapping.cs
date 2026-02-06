using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.InventoryCounts;

public static class InventoryCountMapping
{
    public static InventoryCountDto MapDetail(InventoryCount count)
        => new(
            count.Id,
            count.WarehouseId,
            count.LocationId,
            count.ZoneId,
            count.Warehouse?.Name ?? string.Empty,
            count.Location?.Code ?? string.Empty,
            count.Status,
            count.Notes,
            count.StartedAtUtc,
            count.CompletedAtUtc,
            count.Items.Select(MapItem).ToList());

    public static InventoryCountListItemDto MapListItem(InventoryCount count)
        => new(
            count.Id,
            count.WarehouseId,
            count.LocationId,
            count.Warehouse?.Name ?? string.Empty,
            count.Location?.Code ?? string.Empty,
            count.Status,
            count.Items.Count,
            count.CreatedAtUtc,
            count.IsActive);

    public static InventoryCountItemDto MapItem(InventoryCountItem item)
        => new(
            item.Id,
            item.LocationId,
            item.ProductId,
            item.UomId,
            item.LotId,
            item.Location?.Code ?? string.Empty,
            item.Product?.Code ?? string.Empty,
            item.Product?.Name ?? string.Empty,
            item.Uom?.Code ?? string.Empty,
            item.Lot?.Code,
            item.QuantityExpected,
            item.QuantityCounted);
}
