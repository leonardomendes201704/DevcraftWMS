using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrderNotifications.Commands.ResendOutboundOrderNotification;

public sealed record ResendOutboundOrderNotificationCommand(Guid OutboundOrderId, Guid NotificationId)
    : IRequest<RequestResult<OutboundOrderNotificationDto>>;
