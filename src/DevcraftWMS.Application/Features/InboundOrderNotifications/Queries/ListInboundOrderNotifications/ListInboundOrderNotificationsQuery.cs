using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrderNotifications.Queries.ListInboundOrderNotifications;

public sealed record ListInboundOrderNotificationsQuery(Guid InboundOrderId)
    : IRequest<RequestResult<IReadOnlyList<InboundOrderNotificationDto>>>;
