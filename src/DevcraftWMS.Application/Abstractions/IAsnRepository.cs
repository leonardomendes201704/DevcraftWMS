using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IAsnRepository
{
    Task<bool> AsnNumberExistsAsync(string asnNumber, CancellationToken cancellationToken = default);
    Task<bool> AsnNumberExistsAsync(string asnNumber, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Asn asn, CancellationToken cancellationToken = default);
    Task UpdateAsync(Asn asn, CancellationToken cancellationToken = default);
    Task<Asn?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Asn?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        string? asnNumber,
        string? supplierName,
        string? documentNumber,
        AsnStatus? status,
        DateOnly? expectedFrom,
        DateOnly? expectedTo,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asn>> ListAsync(
        Guid? warehouseId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? asnNumber,
        string? supplierName,
        string? documentNumber,
        AsnStatus? status,
        DateOnly? expectedFrom,
        DateOnly? expectedTo,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
