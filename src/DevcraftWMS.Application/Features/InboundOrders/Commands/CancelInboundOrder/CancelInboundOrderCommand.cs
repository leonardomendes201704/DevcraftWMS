using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.CancelInboundOrder;

public sealed record CancelInboundOrderCommand(Guid Id, string Reason) : IRequest<RequestResult<InboundOrderDetailDto>>;
