using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IReturnOrderRepository
{
    Task AddAsync(ReturnOrder order, CancellationToken cancellationToken = default);
    Task AddItemAsync(ReturnItem item, CancellationToken cancellationToken = default);
    Task<ReturnOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReturnOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(ReturnOrder order, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        string? returnNumber,
        ReturnStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReturnOrder>> ListAsync(
        Guid? warehouseId,
        string? returnNumber,
        ReturnStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
    Task<bool> ReturnNumberExistsAsync(string returnNumber, CancellationToken cancellationToken = default);
}
