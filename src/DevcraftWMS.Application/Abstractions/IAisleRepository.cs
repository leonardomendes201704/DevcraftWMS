using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IAisleRepository
{
    Task<bool> CodeExistsAsync(Guid sectionId, string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid sectionId, string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Aisle aisle, CancellationToken cancellationToken = default);
    Task UpdateAsync(Aisle aisle, CancellationToken cancellationToken = default);
    Task<Aisle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Aisle?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid sectionId,
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Aisle>> ListAsync(
        Guid sectionId,
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
