using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface ICostCenterRepository
{
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(CostCenter costCenter, CancellationToken cancellationToken = default);
    Task UpdateAsync(CostCenter costCenter, CancellationToken cancellationToken = default);
    Task<CostCenter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CostCenter?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CostCenter>> ListAsync(
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
