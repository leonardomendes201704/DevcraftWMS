using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IOutboundOrderRepository
{
    Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task AddAsync(OutboundOrder order, CancellationToken cancellationToken = default);
    Task AddItemAsync(OutboundOrderItem item, CancellationToken cancellationToken = default);
    Task<OutboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        string? orderNumber,
        OutboundOrderStatus? status,
        OutboundOrderPriority? priority,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboundOrder>> ListAsync(
        Guid? warehouseId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? orderNumber,
        OutboundOrderStatus? status,
        OutboundOrderPriority? priority,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
