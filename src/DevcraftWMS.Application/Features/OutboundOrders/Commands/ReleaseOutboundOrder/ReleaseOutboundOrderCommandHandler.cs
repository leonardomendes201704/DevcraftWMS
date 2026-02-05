using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Commands.ReleaseOutboundOrder;

public sealed class ReleaseOutboundOrderCommandHandler
    : IRequestHandler<ReleaseOutboundOrderCommand, RequestResult<OutboundOrderDetailDto>>
{
    private readonly IOutboundOrderService _service;

    public ReleaseOutboundOrderCommandHandler(IOutboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<OutboundOrderDetailDto>> Handle(
        ReleaseOutboundOrderCommand request,
        CancellationToken cancellationToken)
        => _service.ReleaseAsync(
            request.Id,
            request.Priority,
            request.PickingMethod,
            request.ShippingWindowStartUtc,
            request.ShippingWindowEndUtc,
            cancellationToken);
}
