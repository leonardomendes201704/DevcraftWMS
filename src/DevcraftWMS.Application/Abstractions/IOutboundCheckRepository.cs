using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IOutboundCheckRepository
{
    Task AddAsync(OutboundCheck check, CancellationToken cancellationToken = default);
    Task<OutboundCheck?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OutboundCheck?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(OutboundCheck check, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        Guid? outboundOrderId,
        OutboundCheckStatus? status,
        OutboundOrderPriority? priority,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboundCheck>> ListAsync(
        Guid? warehouseId,
        Guid? outboundOrderId,
        OutboundCheckStatus? status,
        OutboundOrderPriority? priority,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default);
}
