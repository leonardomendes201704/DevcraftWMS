using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Queries.GetOutboundOrder;

public sealed record GetOutboundOrderQuery(Guid Id)
    : IRequest<RequestResult<OutboundOrderDetailDto>>;
