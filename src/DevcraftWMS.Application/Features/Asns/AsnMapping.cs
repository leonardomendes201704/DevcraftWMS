using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Asns;

public static class AsnMapping
{
    public static AsnListItemDto MapListItem(Asn asn)
        => new(
            asn.Id,
            asn.AsnNumber,
            asn.DocumentNumber,
            asn.SupplierName,
            asn.WarehouseId,
            asn.Warehouse?.Name ?? string.Empty,
            asn.Status,
            asn.ExpectedArrivalDate,
            asn.Items.Count,
            asn.IsActive,
            asn.CreatedAtUtc);

    public static AsnDetailDto MapDetail(Asn asn)
        => new(
            asn.Id,
            asn.CustomerId,
            asn.WarehouseId,
            asn.Warehouse?.Name ?? string.Empty,
            asn.AsnNumber,
            asn.DocumentNumber,
            asn.SupplierName,
            asn.Status,
            asn.ExpectedArrivalDate,
            asn.Notes,
            asn.IsActive,
            asn.CreatedAtUtc,
            asn.UpdatedAtUtc);

    public static AsnItemDto MapItem(AsnItem item)
        => new(
            item.Id,
            item.ProductId,
            item.Product?.Code ?? string.Empty,
            item.Product?.Name ?? string.Empty,
            item.UomId,
            item.Uom?.Code ?? string.Empty,
            item.Quantity,
            item.LotCode,
            item.ExpirationDate,
            item.IsActive,
            item.CreatedAtUtc);

    public static AsnAttachmentDto MapAttachment(AsnAttachment attachment)
        => new(
            attachment.Id,
            attachment.AsnId,
            attachment.FileName,
            attachment.ContentType,
            attachment.SizeBytes,
            attachment.CreatedAtUtc);

    public static AsnStatusEventDto MapStatusEvent(AsnStatusEvent statusEvent)
        => new(
            statusEvent.Id,
            statusEvent.AsnId,
            statusEvent.FromStatus,
            statusEvent.ToStatus,
            statusEvent.Notes,
            statusEvent.CreatedAtUtc);
}
