using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrderNotifications.Queries.ListInboundOrderNotifications;

public sealed class ListInboundOrderNotificationsQueryHandler
    : IRequestHandler<ListInboundOrderNotificationsQuery, RequestResult<IReadOnlyList<InboundOrderNotificationDto>>>
{
    private readonly IInboundOrderNotificationService _service;

    public ListInboundOrderNotificationsQueryHandler(IInboundOrderNotificationService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<InboundOrderNotificationDto>>> Handle(
        ListInboundOrderNotificationsQuery request,
        CancellationToken cancellationToken)
        => _service.ListAsync(request.InboundOrderId, cancellationToken);
}
