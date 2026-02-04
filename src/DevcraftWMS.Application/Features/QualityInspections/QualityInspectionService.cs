using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.QualityInspections;

public sealed class QualityInspectionService : IQualityInspectionService
{
    private readonly IQualityInspectionRepository _inspectionRepository;
    private readonly ILotRepository _lotRepository;
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService? _currentUserService;

    public QualityInspectionService(
        IQualityInspectionRepository inspectionRepository,
        ILotRepository lotRepository,
        IInventoryBalanceRepository balanceRepository,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService? currentUserService = null)
    {
        _inspectionRepository = inspectionRepository;
        _lotRepository = lotRepository;
        _balanceRepository = balanceRepository;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<RequestResult<PagedResult<QualityInspectionListItemDto>>> ListPagedAsync(
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
        CancellationToken cancellationToken)
    {
        var total = await _inspectionRepository.CountAsync(
            status,
            warehouseId,
            receiptId,
            productId,
            lotId,
            createdFromUtc,
            createdToUtc,
            isActive,
            includeInactive,
            cancellationToken);

        var items = await _inspectionRepository.ListAsync(
            status,
            warehouseId,
            receiptId,
            productId,
            lotId,
            createdFromUtc,
            createdToUtc,
            isActive,
            includeInactive,
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            cancellationToken);

        var mapped = items.Select(QualityInspectionMapping.MapListItem).ToList();
        var result = new PagedResult<QualityInspectionListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<QualityInspectionListItemDto>>.Success(result);
    }

    public async Task<RequestResult<QualityInspectionDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionRepository.GetByIdAsync(id, cancellationToken);
        if (inspection is null)
        {
            return RequestResult<QualityInspectionDetailDto>.Failure("quality.inspection.not_found", "Quality inspection not found.");
        }

        return RequestResult<QualityInspectionDetailDto>.Success(QualityInspectionMapping.MapDetail(inspection));
    }

    public async Task<RequestResult<QualityInspectionDetailDto>> ApproveAsync(Guid id, string? notes, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (inspection is null)
        {
            return RequestResult<QualityInspectionDetailDto>.Failure("quality.inspection.not_found", "Quality inspection not found.");
        }

        if (inspection.Status != QualityInspectionStatus.Pending)
        {
            return RequestResult<QualityInspectionDetailDto>.Failure("quality.inspection.status_locked", "Quality inspection status does not allow approval.");
        }

        if (inspection.LotId.HasValue)
        {
            var lot = await _lotRepository.GetTrackedByIdAsync(inspection.LotId.Value, cancellationToken);
            if (lot is not null && lot.Status == LotStatus.Quarantined)
            {
                lot.Status = LotStatus.Available;
                lot.QuarantineReason = null;
                lot.QuarantinedAtUtc = null;
                await _lotRepository.UpdateAsync(lot, cancellationToken);

                var balances = await _balanceRepository.ListByLotAsync(lot.Id, InventoryBalanceStatus.Blocked, null, true, cancellationToken);
                foreach (var balance in balances)
                {
                    var tracked = await _balanceRepository.GetTrackedByIdAsync(balance.Id, cancellationToken);
                    if (tracked is null)
                    {
                        continue;
                    }

                    tracked.Status = InventoryBalanceStatus.Available;
                    await _balanceRepository.UpdateAsync(tracked, cancellationToken);
                }
            }
        }

        inspection.Status = QualityInspectionStatus.Approved;
        inspection.DecisionNotes = notes;
        inspection.DecisionAtUtc = _dateTimeProvider.UtcNow;
        inspection.DecisionByUserId = _currentUserService?.UserId;

        await _inspectionRepository.UpdateAsync(inspection, cancellationToken);

        return RequestResult<QualityInspectionDetailDto>.Success(QualityInspectionMapping.MapDetail(inspection));
    }

    public async Task<RequestResult<QualityInspectionDetailDto>> RejectAsync(Guid id, string? notes, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (inspection is null)
        {
            return RequestResult<QualityInspectionDetailDto>.Failure("quality.inspection.not_found", "Quality inspection not found.");
        }

        if (inspection.Status != QualityInspectionStatus.Pending)
        {
            return RequestResult<QualityInspectionDetailDto>.Failure("quality.inspection.status_locked", "Quality inspection status does not allow rejection.");
        }

        inspection.Status = QualityInspectionStatus.Rejected;
        inspection.DecisionNotes = notes;
        inspection.DecisionAtUtc = _dateTimeProvider.UtcNow;
        inspection.DecisionByUserId = _currentUserService?.UserId;

        await _inspectionRepository.UpdateAsync(inspection, cancellationToken);

        return RequestResult<QualityInspectionDetailDto>.Success(QualityInspectionMapping.MapDetail(inspection));
    }

    public async Task<RequestResult<QualityInspectionEvidenceDto>> AddEvidenceAsync(Guid inspectionId, string fileName, string contentType, byte[] content, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionRepository.GetByIdAsync(inspectionId, cancellationToken);
        if (inspection is null)
        {
            return RequestResult<QualityInspectionEvidenceDto>.Failure("quality.inspection.not_found", "Quality inspection not found.");
        }

        var evidence = new QualityInspectionEvidence
        {
            Id = Guid.NewGuid(),
            QualityInspectionId = inspectionId,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = content.Length,
            Content = content
        };

        await _inspectionRepository.AddEvidenceAsync(evidence, cancellationToken);
        return RequestResult<QualityInspectionEvidenceDto>.Success(QualityInspectionMapping.MapEvidence(evidence));
    }

    public async Task<RequestResult<IReadOnlyList<QualityInspectionEvidenceDto>>> ListEvidenceAsync(Guid inspectionId, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionRepository.GetByIdAsync(inspectionId, cancellationToken);
        if (inspection is null)
        {
            return RequestResult<IReadOnlyList<QualityInspectionEvidenceDto>>.Failure("quality.inspection.not_found", "Quality inspection not found.");
        }

        var evidence = await _inspectionRepository.ListEvidenceAsync(inspectionId, cancellationToken);
        var mapped = evidence.Select(QualityInspectionMapping.MapEvidence).ToList();
        return RequestResult<IReadOnlyList<QualityInspectionEvidenceDto>>.Success(mapped);
    }

    public async Task<RequestResult<QualityInspectionEvidenceContentDto>> GetEvidenceAsync(Guid inspectionId, Guid evidenceId, CancellationToken cancellationToken)
    {
        var evidence = await _inspectionRepository.GetEvidenceByIdAsync(evidenceId, cancellationToken);
        if (evidence is null || evidence.QualityInspectionId != inspectionId)
        {
            return RequestResult<QualityInspectionEvidenceContentDto>.Failure("quality.inspection.evidence.not_found", "Evidence not found.");
        }

        return RequestResult<QualityInspectionEvidenceContentDto>.Success(QualityInspectionMapping.MapEvidenceContent(evidence));
    }
}
