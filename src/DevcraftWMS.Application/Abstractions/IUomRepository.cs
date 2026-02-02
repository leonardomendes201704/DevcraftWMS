using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IUomRepository
{
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Uom uom, CancellationToken cancellationToken = default);
    Task UpdateAsync(Uom uom, CancellationToken cancellationToken = default);
    Task<Uom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Uom?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        string? code,
        string? name,
        UomType? type,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Uom>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        UomType? type,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
