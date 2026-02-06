using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IPickingReplenishmentTaskRepository
{
    Task AddRangeAsync(IReadOnlyList<PickingReplenishmentTask> tasks, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        Guid? productId,
        PickingReplenishmentStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PickingReplenishmentTask>> ListAsync(
        Guid? warehouseId,
        Guid? productId,
        PickingReplenishmentStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsOpenTaskAsync(Guid productId, Guid toLocationId, CancellationToken cancellationToken = default);
}
