using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundShipping.Commands.RegisterOutboundShipment;

public sealed record RegisterOutboundShipmentCommand(
    Guid OutboundOrderId,
    RegisterOutboundShipmentInput Input)
    : IRequest<RequestResult<OutboundShipmentDto>>;
