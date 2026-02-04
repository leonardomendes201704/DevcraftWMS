using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrderNotifications.Commands.ResendInboundOrderNotification;

public sealed record ResendInboundOrderNotificationCommand(Guid InboundOrderId, Guid NotificationId)
    : IRequest<RequestResult<InboundOrderNotificationDto>>;
