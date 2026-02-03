using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Queries.GetInboundOrder;

public sealed record GetInboundOrderQuery(Guid Id) : IRequest<RequestResult<InboundOrderDetailDto>>;
