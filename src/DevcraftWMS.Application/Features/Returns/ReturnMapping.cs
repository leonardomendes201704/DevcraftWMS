using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Returns;

public static class ReturnMapping
{
    public static ReturnOrderDto MapDetail(ReturnOrder order)
        => new(
            order.Id,
            order.WarehouseId,
            order.OutboundOrderId,
            order.ReturnNumber,
            order.Warehouse?.Name ?? string.Empty,
            order.OutboundOrder?.OrderNumber,
            order.Status,
            order.Notes,
            order.ReceivedAtUtc,
            order.CompletedAtUtc,
            order.Items.Select(MapItem).ToList());

    public static ReturnOrderListItemDto MapListItem(ReturnOrder order)
        => new(
            order.Id,
            order.WarehouseId,
            order.ReturnNumber,
            order.Warehouse?.Name ?? string.Empty,
            order.Status,
            order.Items.Count,
            order.CreatedAtUtc,
            order.IsActive);

    public static ReturnItemDto MapItem(ReturnItem item)
        => new(
            item.Id,
            item.ProductId,
            item.UomId,
            item.LotId,
            item.Product?.Code ?? string.Empty,
            item.Product?.Name ?? string.Empty,
            item.Uom?.Code ?? string.Empty,
            item.LotCode,
            item.ExpirationDate,
            item.QuantityExpected,
            item.QuantityReceived,
            item.Disposition,
            item.DispositionNotes);
}
