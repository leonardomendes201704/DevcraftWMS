using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IWarehouseRepository
{
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
    Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        string? search,
        string? code,
        string? name,
        WarehouseType? warehouseType,
        string? city,
        string? state,
        string? country,
        string? externalId,
        string? erpCode,
        string? costCenterCode,
        bool? isPrimary,
        bool includeInactive,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Warehouse>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? search,
        string? code,
        string? name,
        WarehouseType? warehouseType,
        string? city,
        string? state,
        string? country,
        string? externalId,
        string? erpCode,
        string? costCenterCode,
        bool? isPrimary,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
