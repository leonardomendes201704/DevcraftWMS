using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.ReceiptDivergences;

public sealed class ReceiptDivergenceOptions
{
    public const string SectionName = "ReceiptDivergences";

    public IReadOnlyList<ReceiptDivergenceType> EvidenceRequiredTypes { get; init; } = Array.Empty<ReceiptDivergenceType>();
    public long MaxEvidenceBytes { get; init; } = 5_000_000;
}
