using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IPutawayTaskRepository
{
    Task AddAsync(PutawayTask task, CancellationToken cancellationToken = default);
    Task UpdateAsync(PutawayTask task, CancellationToken cancellationToken = default);
    Task<PutawayTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PutawayTask?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUnitLoadIdAsync(Guid unitLoadId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Guid? warehouseId, Guid? receiptId, Guid? unitLoadId, PutawayTaskStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PutawayTask>> ListAsync(
        Guid? warehouseId,
        Guid? receiptId,
        Guid? unitLoadId,
        PutawayTaskStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
}
