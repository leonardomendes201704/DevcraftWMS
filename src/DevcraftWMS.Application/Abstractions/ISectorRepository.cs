using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface ISectorRepository
{
    Task<bool> CodeExistsAsync(Guid warehouseId, string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid warehouseId, string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Sector sector, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sector sector, CancellationToken cancellationToken = default);
    Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sector?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        string? code,
        string? name,
        SectorType? sectorType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Sector>> ListAsync(
        Guid? warehouseId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        SectorType? sectorType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
