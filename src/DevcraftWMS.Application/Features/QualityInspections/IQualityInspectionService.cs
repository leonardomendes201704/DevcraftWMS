using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.QualityInspections;

public interface IQualityInspectionService
{
    Task<RequestResult<PagedResult<QualityInspectionListItemDto>>> ListPagedAsync(
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
        CancellationToken cancellationToken);

    Task<RequestResult<QualityInspectionDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<RequestResult<QualityInspectionDetailDto>> ApproveAsync(Guid id, string? notes, CancellationToken cancellationToken);
    Task<RequestResult<QualityInspectionDetailDto>> RejectAsync(Guid id, string? notes, CancellationToken cancellationToken);
    Task<RequestResult<QualityInspectionEvidenceDto>> AddEvidenceAsync(Guid inspectionId, string fileName, string contentType, byte[] content, CancellationToken cancellationToken);
    Task<RequestResult<IReadOnlyList<QualityInspectionEvidenceDto>>> ListEvidenceAsync(Guid inspectionId, CancellationToken cancellationToken);
    Task<RequestResult<QualityInspectionEvidenceContentDto>> GetEvidenceAsync(Guid inspectionId, Guid evidenceId, CancellationToken cancellationToken);
}
