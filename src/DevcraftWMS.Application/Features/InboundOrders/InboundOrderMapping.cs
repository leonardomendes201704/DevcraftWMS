using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.InboundOrders;

public static class InboundOrderMapping
{
    public static InboundOrderListItemDto MapListItem(InboundOrder order)
        => new(
            order.Id,
            order.OrderNumber,
            order.Asn?.AsnNumber ?? string.Empty,
            order.Warehouse?.Name ?? string.Empty,
            order.Status,
            order.Priority,
            order.ExpectedArrivalDate,
            order.CreatedAtUtc,
            order.IsActive);

    public static InboundOrderDetailDto MapDetail(
        InboundOrder order,
        IReadOnlyList<InboundOrderItemDto> items,
        IReadOnlyList<InboundOrderStatusEventDto> statusEvents)
        => new(
            order.Id,
            order.AsnId,
            order.WarehouseId,
            order.OrderNumber,
            order.Asn?.AsnNumber ?? string.Empty,
            order.Warehouse?.Name ?? string.Empty,
            order.SupplierName,
            order.DocumentNumber,
            order.ExpectedArrivalDate,
            order.Notes,
            order.Status,
            order.Priority,
            order.InspectionLevel,
            order.SuggestedDock,
            order.CancelReason,
            order.CanceledAtUtc,
            order.CreatedAtUtc,
            order.IsActive,
            items,
            statusEvents);

    public static InboundOrderItemDto MapItem(InboundOrderItem item)
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

    public static InboundOrderStatusEventDto MapStatusEvent(InboundOrderStatusEvent statusEvent)
        => new(
            statusEvent.Id,
            statusEvent.FromStatus,
            statusEvent.ToStatus,
            statusEvent.Notes,
            statusEvent.CreatedAtUtc);
}
