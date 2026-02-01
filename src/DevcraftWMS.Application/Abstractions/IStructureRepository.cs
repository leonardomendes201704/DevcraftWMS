using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IStructureRepository
{
    Task<bool> CodeExistsAsync(Guid sectionId, string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid sectionId, string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Structure structure, CancellationToken cancellationToken = default);
    Task UpdateAsync(Structure structure, CancellationToken cancellationToken = default);
    Task<Structure?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Structure?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid sectionId,
        string? code,
        string? name,
        StructureType? structureType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Structure>> ListAsync(
        Guid sectionId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        StructureType? structureType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
