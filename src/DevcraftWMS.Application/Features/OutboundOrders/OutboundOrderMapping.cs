using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.OutboundOrders;

public static class OutboundOrderMapping
{
    public static OutboundOrderListItemDto MapListItem(OutboundOrder order)
        => new(
            order.Id,
            order.OrderNumber,
            order.Warehouse?.Name ?? string.Empty,
            order.Status,
            order.Priority,
            order.ExpectedShipDate,
            order.CreatedAtUtc,
            order.IsActive);

    public static OutboundOrderDetailDto MapDetail(OutboundOrder order, IReadOnlyList<OutboundOrderItemDto> items)
        => new(
            order.Id,
            order.WarehouseId,
            order.OrderNumber,
            order.Warehouse?.Name ?? string.Empty,
            order.CustomerReference,
            order.CarrierName,
            order.ExpectedShipDate,
            order.Notes,
            order.Status,
            order.Priority,
            order.PickingMethod,
            order.ShippingWindowStartUtc,
            order.ShippingWindowEndUtc,
            order.CancelReason,
            order.CanceledAtUtc,
            order.CreatedAtUtc,
            order.IsActive,
            items);

    public static OutboundOrderItemDto MapItem(OutboundOrderItem item)
        => new(
            item.Id,
            item.ProductId,
            item.UomId,
            item.Product?.Code ?? string.Empty,
            item.Product?.Name ?? string.Empty,
            item.Uom?.Code ?? string.Empty,
            item.Quantity,
            item.LotCode,
            item.ExpirationDate);
}
