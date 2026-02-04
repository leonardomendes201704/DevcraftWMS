using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrderNotifications.Commands.ResendInboundOrderNotification;

public sealed class ResendInboundOrderNotificationCommandHandler
    : IRequestHandler<ResendInboundOrderNotificationCommand, RequestResult<InboundOrderNotificationDto>>
{
    private readonly IInboundOrderNotificationService _service;

    public ResendInboundOrderNotificationCommandHandler(IInboundOrderNotificationService service)
    {
        _service = service;
    }

    public Task<RequestResult<InboundOrderNotificationDto>> Handle(
        ResendInboundOrderNotificationCommand request,
        CancellationToken cancellationToken)
        => _service.ResendAsync(request.InboundOrderId, request.NotificationId, cancellationToken);
}
