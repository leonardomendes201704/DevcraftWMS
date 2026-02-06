using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IInventoryBalanceRepository
{
    Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(InventoryBalance balance, CancellationToken cancellationToken = default);
    Task UpdateAsync(InventoryBalance balance, CancellationToken cancellationToken = default);
    Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryBalance?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryBalance?> GetTrackedByKeyAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? locationId,
        Guid? productId,
        Guid? lotId,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryBalance>> ListAsync(
        Guid? locationId,
        Guid? productId,
        Guid? lotId,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
    Task<int> CountByLocationAsync(
        Guid locationId,
        Guid? productId,
        Guid? lotId,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryBalance>> ListByLocationAsync(
        Guid locationId,
        Guid? productId,
        Guid? lotId,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryBalance>> ListByLotAsync(
        Guid lotId,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryBalance>> ListAvailableForReservationAsync(
        Guid productId,
        Guid? lotId,
        ZoneType? zoneType = null,
        CancellationToken cancellationToken = default);
}
