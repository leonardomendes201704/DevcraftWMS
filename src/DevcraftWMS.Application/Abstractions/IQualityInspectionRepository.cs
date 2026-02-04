using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IQualityInspectionRepository
{
    Task AddAsync(QualityInspection inspection, CancellationToken cancellationToken = default);
    Task UpdateAsync(QualityInspection inspection, CancellationToken cancellationToken = default);
    Task<QualityInspection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<QualityInspection?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsOpenForLotAsync(Guid lotId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        QualityInspectionStatus? status,
        Guid? warehouseId,
        Guid? receiptId,
        Guid? productId,
        Guid? lotId,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QualityInspection>> ListAsync(
        QualityInspectionStatus? status,
        Guid? warehouseId,
        Guid? receiptId,
        Guid? productId,
        Guid? lotId,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
    Task AddEvidenceAsync(QualityInspectionEvidence evidence, CancellationToken cancellationToken = default);
    Task<QualityInspectionEvidence?> GetEvidenceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QualityInspectionEvidence>> ListEvidenceAsync(Guid inspectionId, CancellationToken cancellationToken = default);
}
