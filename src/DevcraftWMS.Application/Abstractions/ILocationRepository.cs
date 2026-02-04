using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface ILocationRepository
{
    Task<bool> CodeExistsAsync(Guid structureId, string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid structureId, string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Location location, CancellationToken cancellationToken = default);
    Task UpdateAsync(Location location, CancellationToken cancellationToken = default);
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Location?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid structureId,
        Guid? zoneId,
        string? code,
        string? barcode,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Location>> ListAsync(
        Guid structureId,
        Guid? zoneId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? barcode,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Location>> ListByStructureAsync(
        Guid structureId,
        Guid? zoneId,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
