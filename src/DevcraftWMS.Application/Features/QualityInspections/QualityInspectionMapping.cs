using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.QualityInspections;

public static class QualityInspectionMapping
{
    public static QualityInspectionListItemDto MapListItem(QualityInspection inspection)
        => new(
            inspection.Id,
            inspection.WarehouseId,
            inspection.ReceiptId,
            inspection.ProductId,
            inspection.Product?.Code ?? string.Empty,
            inspection.Product?.Name ?? string.Empty,
            inspection.LotId,
            inspection.Lot?.Code,
            inspection.LocationId,
            inspection.Location?.Code ?? string.Empty,
            inspection.Status,
            inspection.Reason,
            inspection.CreatedAtUtc,
            inspection.DecisionAtUtc,
            inspection.IsActive);

    public static QualityInspectionDetailDto MapDetail(QualityInspection inspection)
        => new(
            inspection.Id,
            inspection.WarehouseId,
            inspection.ReceiptId,
            inspection.ReceiptItemId,
            inspection.ProductId,
            inspection.Product?.Code ?? string.Empty,
            inspection.Product?.Name ?? string.Empty,
            inspection.LotId,
            inspection.Lot?.Code,
            inspection.Lot?.ExpirationDate,
            inspection.LocationId,
            inspection.Location?.Code ?? string.Empty,
            inspection.Status,
            inspection.Reason,
            inspection.Notes,
            inspection.DecisionNotes,
            inspection.CreatedAtUtc,
            inspection.DecisionAtUtc,
            inspection.IsActive);

    public static QualityInspectionEvidenceDto MapEvidence(QualityInspectionEvidence evidence)
        => new(
            evidence.Id,
            evidence.FileName,
            evidence.ContentType,
            evidence.SizeBytes,
            evidence.CreatedAtUtc);

    public static QualityInspectionEvidenceContentDto MapEvidenceContent(QualityInspectionEvidence evidence)
        => new(
            evidence.Id,
            evidence.FileName,
            evidence.ContentType,
            evidence.SizeBytes,
            evidence.Content,
            evidence.CreatedAtUtc);
}
