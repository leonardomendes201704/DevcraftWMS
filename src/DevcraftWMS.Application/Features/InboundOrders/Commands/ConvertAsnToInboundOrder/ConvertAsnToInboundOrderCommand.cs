using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.ConvertAsnToInboundOrder;

public sealed record ConvertAsnToInboundOrderCommand(Guid AsnId, string? Notes) : IRequest<RequestResult<InboundOrderDetailDto>>;
