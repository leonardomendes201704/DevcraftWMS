using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.ReceiptCounts;

public static class ReceiptCountMapping
{
    public static ReceiptExpectedItemDto MapExpectedItem(InboundOrderItem item)
        => new(
            item.Id,
            item.ProductId,
            item.Product?.Code ?? "-",
            item.Product?.Name ?? "-",
            item.UomId,
            item.Uom?.Code ?? "-",
            item.Quantity,
            item.LotCode,
            item.ExpirationDate);

    public static ReceiptCountDto MapCount(ReceiptCount count)
        => new(
            count.Id,
            count.ReceiptId,
            count.InboundOrderItemId,
            count.InboundOrderItem?.Product?.Code ?? "-",
            count.InboundOrderItem?.Product?.Name ?? "-",
            count.InboundOrderItem?.Uom?.Code ?? "-",
            count.ExpectedQuantity,
            count.CountedQuantity,
            count.Variance,
            count.Mode,
            count.Variance != 0,
            count.Notes,
            count.CreatedAtUtc);
}
