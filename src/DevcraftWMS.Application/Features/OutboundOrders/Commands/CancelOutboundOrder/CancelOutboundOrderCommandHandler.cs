using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Commands.CancelOutboundOrder;

public sealed class CancelOutboundOrderCommandHandler
    : IRequestHandler<CancelOutboundOrderCommand, RequestResult<OutboundOrderDetailDto>>
{
    private readonly IOutboundOrderService _service;

    public CancelOutboundOrderCommandHandler(IOutboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<OutboundOrderDetailDto>> Handle(
        CancelOutboundOrderCommand request,
        CancellationToken cancellationToken)
        => _service.CancelAsync(request.Id, request.Reason, cancellationToken);
}
