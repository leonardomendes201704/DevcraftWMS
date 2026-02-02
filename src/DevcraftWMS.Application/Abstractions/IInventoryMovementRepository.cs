using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IInventoryMovementRepository
{
    Task AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default);
    Task UpdateAsync(InventoryMovement movement, CancellationToken cancellationToken = default);
    Task<InventoryMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryMovement?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? productId,
        Guid? fromLocationId,
        Guid? toLocationId,
        Guid? lotId,
        InventoryMovementStatus? status,
        DateTime? performedFromUtc,
        DateTime? performedToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryMovement>> ListAsync(
        Guid? productId,
        Guid? fromLocationId,
        Guid? toLocationId,
        Guid? lotId,
        InventoryMovementStatus? status,
        DateTime? performedFromUtc,
        DateTime? performedToUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);
}
