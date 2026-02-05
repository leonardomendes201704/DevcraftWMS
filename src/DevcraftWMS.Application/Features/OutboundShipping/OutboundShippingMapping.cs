using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.OutboundShipping;

public static class OutboundShippingMapping
{
    public static OutboundShipmentDto Map(OutboundShipment shipment)
        => new(
            shipment.Id,
            shipment.OutboundOrderId,
            shipment.WarehouseId,
            shipment.OutboundOrder?.OrderNumber ?? "-",
            shipment.Warehouse?.Name ?? "-",
            shipment.DockCode,
            shipment.LoadingStartedAtUtc,
            shipment.LoadingCompletedAtUtc,
            shipment.ShippedAtUtc,
            shipment.Notes,
            shipment.Items.Select(MapItem).ToList());

    public static OutboundShipmentItemDto MapItem(OutboundShipmentItem item)
        => new(
            item.Id,
            item.OutboundPackageId,
            item.PackageNumber,
            item.WeightKg);
}
