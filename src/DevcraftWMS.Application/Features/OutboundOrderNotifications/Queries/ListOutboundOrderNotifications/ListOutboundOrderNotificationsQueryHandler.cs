using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrderNotifications.Queries.ListOutboundOrderNotifications;

public sealed class ListOutboundOrderNotificationsQueryHandler
    : IRequestHandler<ListOutboundOrderNotificationsQuery, RequestResult<IReadOnlyList<OutboundOrderNotificationDto>>>
{
    private readonly IOutboundOrderNotificationService _service;

    public ListOutboundOrderNotificationsQueryHandler(IOutboundOrderNotificationService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<OutboundOrderNotificationDto>>> Handle(
        ListOutboundOrderNotificationsQuery request,
        CancellationToken cancellationToken)
        => _service.ListAsync(request.OutboundOrderId, cancellationToken);
}
