using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrderNotifications.Queries.ListOutboundOrderNotifications;

public sealed record ListOutboundOrderNotificationsQuery(Guid OutboundOrderId)
    : IRequest<RequestResult<IReadOnlyList<OutboundOrderNotificationDto>>>;
