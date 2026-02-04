using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IUnitLoadRepository
{
    Task AddAsync(UnitLoad unitLoad, CancellationToken cancellationToken = default);
    Task UpdateAsync(UnitLoad unitLoad, CancellationToken cancellationToken = default);
    Task<UnitLoad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UnitLoad?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddRelabelEventAsync(UnitLoadRelabelEvent relabelEvent, CancellationToken cancellationToken = default);
    Task<bool> SsccExistsAsync(string ssccInternal, CancellationToken cancellationToken = default);
    Task<bool> AnyNotPutawayCompletedByReceiptIdsAsync(IReadOnlyCollection<Guid> receiptIds, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        Guid? receiptId,
        string? sscc,
        UnitLoadStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UnitLoad>> ListAsync(
        Guid? warehouseId,
        Guid? receiptId,
        string? sscc,
        UnitLoadStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
}
