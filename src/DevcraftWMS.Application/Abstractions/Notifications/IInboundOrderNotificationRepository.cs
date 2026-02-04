using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions.Notifications;

public interface IInboundOrderNotificationRepository
{
    Task AddAsync(InboundOrderNotification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(InboundOrderNotification notification, CancellationToken cancellationToken = default);
    Task<InboundOrderNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InboundOrderNotification>> ListByInboundOrderIdAsync(Guid inboundOrderId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid inboundOrderId, string eventType, InboundOrderNotificationChannel channel, CancellationToken cancellationToken = default);
}
