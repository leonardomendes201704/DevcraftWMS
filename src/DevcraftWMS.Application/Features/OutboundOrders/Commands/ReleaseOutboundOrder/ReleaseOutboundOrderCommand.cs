using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Commands.ReleaseOutboundOrder;

public sealed record ReleaseOutboundOrderCommand(
    Guid Id,
    OutboundOrderPriority Priority,
    OutboundOrderPickingMethod PickingMethod,
    DateTime? ShippingWindowStartUtc,
    DateTime? ShippingWindowEndUtc) : IRequest<RequestResult<OutboundOrderDetailDto>>;
