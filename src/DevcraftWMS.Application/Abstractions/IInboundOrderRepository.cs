using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IInboundOrderRepository
{
    Task AddAsync(InboundOrder order, CancellationToken cancellationToken = default);
    Task UpdateAsync(InboundOrder order, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByAsnAsync(Guid asnId, CancellationToken cancellationToken = default);
    Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<InboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InboundOrder?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task<InboundOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        string? orderNumber,
        InboundOrderStatus? status,
        InboundOrderPriority? priority,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InboundOrder>> ListAsync(
        Guid? warehouseId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? orderNumber,
        InboundOrderStatus? status,
        InboundOrderPriority? priority,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task AddItemAsync(InboundOrderItem item, CancellationToken cancellationToken = default);
    Task<InboundOrderItem?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InboundOrderItem>> ListItemsAsync(Guid inboundOrderId, CancellationToken cancellationToken = default);
}
