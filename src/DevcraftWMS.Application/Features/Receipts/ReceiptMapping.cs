using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Receipts;

public static class ReceiptMapping
{
    public static ReceiptListItemDto MapListItem(Receipt receipt)
        => new(
            receipt.Id,
            receipt.InboundOrderId,
            receipt.InboundOrder?.OrderNumber,
            receipt.ReceiptNumber,
            receipt.DocumentNumber,
            receipt.SupplierName,
            receipt.WarehouseId,
            receipt.Warehouse?.Name ?? "-",
            receipt.Status,
            receipt.StartedAtUtc,
            receipt.ReceivedAtUtc,
            receipt.Items.Count,
            receipt.IsActive,
            receipt.CreatedAtUtc);

    public static ReceiptDetailDto MapDetail(Receipt receipt)
        => new(
            receipt.Id,
            receipt.CustomerId,
            receipt.WarehouseId,
            receipt.Warehouse?.Name ?? "-",
            receipt.InboundOrderId,
            receipt.InboundOrder?.OrderNumber,
            receipt.ReceiptNumber,
            receipt.DocumentNumber,
            receipt.SupplierName,
            receipt.Status,
            receipt.StartedAtUtc,
            receipt.ReceivedAtUtc,
            receipt.Notes,
            receipt.IsActive,
            receipt.CreatedAtUtc,
            receipt.UpdatedAtUtc);

    public static ReceiptItemDto MapItem(ReceiptItem item)
        => new(
            item.Id,
            item.ProductId,
            item.Product?.Code ?? "-",
            item.Product?.Name ?? "-",
            item.LotId,
            item.Lot?.Code,
            item.LocationId,
            item.Location?.Code ?? "-",
            item.UomId,
            item.Uom?.Code ?? "-",
            item.Quantity,
            item.UnitCost,
            item.IsActive,
            item.CreatedAtUtc);
}
