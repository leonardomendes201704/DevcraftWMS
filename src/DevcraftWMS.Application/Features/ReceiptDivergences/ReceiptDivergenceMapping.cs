using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.ReceiptDivergences;

public static class ReceiptDivergenceMapping
{
    public static ReceiptDivergenceDto Map(ReceiptDivergence divergence)
    {
        var product = divergence.InboundOrderItem?.Product;
        return new ReceiptDivergenceDto(
            divergence.Id,
            divergence.ReceiptId,
            divergence.InboundOrderId,
            divergence.InboundOrderItemId,
            product?.Code,
            product?.Name,
            divergence.Type,
            divergence.Notes,
            divergence.RequiresEvidence,
            divergence.Evidence.Count,
            divergence.CreatedAtUtc);
    }

    public static ReceiptDivergenceEvidenceDto MapEvidence(ReceiptDivergenceEvidence evidence)
        => new(
            evidence.Id,
            evidence.ReceiptDivergenceId,
            evidence.FileName,
            evidence.ContentType,
            evidence.SizeBytes,
            evidence.CreatedAtUtc);
}
