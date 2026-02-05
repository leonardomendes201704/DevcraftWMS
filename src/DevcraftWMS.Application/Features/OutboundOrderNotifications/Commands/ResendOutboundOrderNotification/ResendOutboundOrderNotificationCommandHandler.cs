using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrderNotifications.Commands.ResendOutboundOrderNotification;

public sealed class ResendOutboundOrderNotificationCommandHandler
    : IRequestHandler<ResendOutboundOrderNotificationCommand, RequestResult<OutboundOrderNotificationDto>>
{
    private readonly IOutboundOrderNotificationService _service;

    public ResendOutboundOrderNotificationCommandHandler(IOutboundOrderNotificationService service)
    {
        _service = service;
    }

    public Task<RequestResult<OutboundOrderNotificationDto>> Handle(
        ResendOutboundOrderNotificationCommand request,
        CancellationToken cancellationToken)
        => _service.ResendAsync(request.OutboundOrderId, request.NotificationId, cancellationToken);
}
