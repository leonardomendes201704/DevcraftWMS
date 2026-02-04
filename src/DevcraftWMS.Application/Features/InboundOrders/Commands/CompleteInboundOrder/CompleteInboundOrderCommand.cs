using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.CompleteInboundOrder;

public sealed record CompleteInboundOrderCommand(Guid Id, bool AllowPartial, string? Notes)
    : IRequest<RequestResult<InboundOrderDetailDto>>;
