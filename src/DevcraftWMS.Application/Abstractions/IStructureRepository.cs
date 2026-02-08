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
        Guid? warehouseId,
        Guid? sectorId,
        Guid? sectionId,
        string? code,
        string? name,
        StructureType? structureType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<int> CountForCustomerAsync(
        Guid? warehouseId,
        Guid? sectorId,
        Guid? sectionId,
        string? code,
        string? name,
        StructureType? structureType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Structure>> ListAsync(
        Guid? warehouseId,
        Guid? sectorId,
        Guid? sectionId,
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
    Task<IReadOnlyList<Structure>> ListForCustomerAsync(
        Guid? warehouseId,
        Guid? sectorId,
        Guid? sectionId,
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
    Task<IReadOnlyList<Structure>> ListByWarehouseAsync(
        Guid warehouseId,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
