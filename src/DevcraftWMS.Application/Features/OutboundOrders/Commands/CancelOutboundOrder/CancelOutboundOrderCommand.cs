using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Commands.CancelOutboundOrder;

public sealed record CancelOutboundOrderCommand(Guid Id, string Reason)
    : IRequest<RequestResult<OutboundOrderDetailDto>>;
