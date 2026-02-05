using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IPickingTaskRepository
{
    Task AddRangeAsync(IReadOnlyList<PickingTask> tasks, CancellationToken cancellationToken = default);
    Task UpdateAsync(PickingTask task, CancellationToken cancellationToken = default);
    Task<PickingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PickingTask?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        Guid? outboundOrderId,
        Guid? assignedUserId,
        PickingTaskStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PickingTask>> ListAsync(
        Guid? warehouseId,
        Guid? outboundOrderId,
        Guid? assignedUserId,
        PickingTaskStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
}
