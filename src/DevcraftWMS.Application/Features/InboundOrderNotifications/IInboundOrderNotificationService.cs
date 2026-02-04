using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InboundOrderNotifications;

public interface IInboundOrderNotificationService
{
    Task<RequestResult<IReadOnlyList<InboundOrderNotificationDto>>> ListAsync(Guid inboundOrderId, CancellationToken cancellationToken);
    Task<RequestResult<InboundOrderNotificationDto>> ResendAsync(Guid inboundOrderId, Guid notificationId, CancellationToken cancellationToken);
    Task NotifyCompletionAsync(InboundOrder order, InboundOrderStatus targetStatus, CancellationToken cancellationToken);
}
