using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface ILotRepository
{
    Task<bool> CodeExistsAsync(Guid productId, string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid productId, string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Lot lot, CancellationToken cancellationToken = default);
    Task UpdateAsync(Lot lot, CancellationToken cancellationToken = default);
    Task<Lot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Lot?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid productId,
        string? code,
        LotStatus? status,
        DateOnly? expirationFrom,
        DateOnly? expirationTo,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Lot>> ListAsync(
        Guid productId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        LotStatus? status,
        DateOnly? expirationFrom,
        DateOnly? expirationTo,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);

    Task<int> CountExpiringAsync(
        DateOnly expirationFrom,
        DateOnly expirationTo,
        LotStatus? status,
        CancellationToken cancellationToken = default);
}
