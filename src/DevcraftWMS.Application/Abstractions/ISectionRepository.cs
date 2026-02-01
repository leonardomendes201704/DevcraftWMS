using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface ISectionRepository
{
    Task<bool> CodeExistsAsync(Guid sectorId, string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid sectorId, string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Section section, CancellationToken cancellationToken = default);
    Task UpdateAsync(Section section, CancellationToken cancellationToken = default);
    Task<Section?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Section?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid sectorId,
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Section>> ListAsync(
        Guid sectorId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
