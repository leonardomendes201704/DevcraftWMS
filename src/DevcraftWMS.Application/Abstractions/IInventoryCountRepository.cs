using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IInventoryCountRepository
{
    Task AddAsync(InventoryCount count, CancellationToken cancellationToken = default);
    Task AddItemAsync(InventoryCountItem item, CancellationToken cancellationToken = default);
    Task<InventoryCount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryCount?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(InventoryCount count, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        Guid? locationId,
        InventoryCountStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryCount>> ListAsync(
        Guid? warehouseId,
        Guid? locationId,
        InventoryCountStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
}
