using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IZoneRepository
{
    Task<bool> CodeExistsAsync(Guid warehouseId, string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid warehouseId, string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Zone zone, CancellationToken cancellationToken = default);
    Task UpdateAsync(Zone zone, CancellationToken cancellationToken = default);
    Task<Zone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Zone?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid warehouseId,
        string? code,
        string? name,
        ZoneType? zoneType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Zone>> ListAsync(
        Guid warehouseId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        ZoneType? zoneType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
