using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IReceiptDivergenceRepository
{
    Task AddAsync(ReceiptDivergence divergence, CancellationToken cancellationToken = default);
    Task AddEvidenceAsync(ReceiptDivergenceEvidence evidence, CancellationToken cancellationToken = default);
    Task<ReceiptDivergence?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReceiptDivergence>> ListByReceiptAsync(Guid receiptId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReceiptDivergence>> ListByInboundOrderAsync(Guid inboundOrderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReceiptDivergenceEvidence>> ListEvidenceAsync(Guid divergenceId, CancellationToken cancellationToken = default);
    Task<ReceiptDivergenceEvidence?> GetEvidenceByIdAsync(Guid divergenceId, Guid evidenceId, CancellationToken cancellationToken = default);
}
