using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.OutboundChecks;

public static class OutboundCheckMapping
{
    public static OutboundCheckDto Map(OutboundCheck check)
        => new(
            check.Id,
            check.OutboundOrderId,
            check.WarehouseId,
            check.OutboundOrder?.OrderNumber ?? "-",
            check.Warehouse?.Name ?? "-",
            check.CheckedByUserId,
            check.CheckedAtUtc,
            check.Notes,
            check.Items.Select(MapItem).ToList());

    public static OutboundCheckItemDto MapItem(OutboundCheckItem item)
        => new(
            item.Id,
            item.OutboundOrderItemId,
            item.ProductId,
            item.UomId,
            item.Product?.Code ?? "-",
            item.Product?.Name ?? "-",
            item.Uom?.Code ?? "-",
            item.QuantityExpected,
            item.QuantityChecked,
            item.DivergenceReason,
            item.Evidence.Count);

    public static OutboundCheckEvidenceDto MapEvidence(OutboundCheckEvidence evidence)
        => new(
            evidence.Id,
            evidence.OutboundCheckItemId,
            evidence.FileName,
            evidence.ContentType,
            evidence.SizeBytes,
            evidence.CreatedAtUtc);
}
