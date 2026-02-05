using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.OutboundOrderNotifications;

public interface IOutboundOrderNotificationService
{
    Task NotifyShipmentAsync(OutboundOrder order, OutboundOrderStatus targetStatus, CancellationToken cancellationToken);
    Task<RequestResult<IReadOnlyList<OutboundOrderNotificationDto>>> ListAsync(Guid outboundOrderId, CancellationToken cancellationToken);
    Task<RequestResult<OutboundOrderNotificationDto>> ResendAsync(Guid outboundOrderId, Guid notificationId, CancellationToken cancellationToken);
}
