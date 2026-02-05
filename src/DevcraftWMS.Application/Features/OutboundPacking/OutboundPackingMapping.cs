using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.OutboundPacking;

public static class OutboundPackingMapping
{
    public static OutboundPackageDto Map(OutboundPackage package)
        => new(
            package.Id,
            package.OutboundOrderId,
            package.WarehouseId,
            package.OutboundOrder?.OrderNumber ?? "-",
            package.Warehouse?.Name ?? "-",
            package.PackageNumber,
            package.WeightKg,
            package.LengthCm,
            package.WidthCm,
            package.HeightCm,
            package.PackedAtUtc,
            package.PackedByUserId,
            package.Notes,
            package.Items.Select(MapItem).ToList());

    public static OutboundPackageItemDto MapItem(OutboundPackageItem item)
        => new(
            item.Id,
            item.OutboundOrderItemId,
            item.ProductId,
            item.UomId,
            item.Product?.Code ?? "-",
            item.Product?.Name ?? "-",
            item.Uom?.Code ?? "-",
            item.Quantity);
}
