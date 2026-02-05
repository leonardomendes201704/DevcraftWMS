using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions.Notifications;

public interface IOutboundOrderNotificationRepository
{
    Task AddAsync(OutboundOrderNotification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(OutboundOrderNotification notification, CancellationToken cancellationToken = default);
    Task<OutboundOrderNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboundOrderNotification>> ListByOutboundOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid outboundOrderId, string eventType, OutboundOrderNotificationChannel channel, CancellationToken cancellationToken = default);
}
