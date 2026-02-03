using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.CancelInboundOrder;

public sealed class CancelInboundOrderCommandHandler : IRequestHandler<CancelInboundOrderCommand, RequestResult<InboundOrderDetailDto>>
{
    private readonly IInboundOrderService _service;

    public CancelInboundOrderCommandHandler(IInboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<InboundOrderDetailDto>> Handle(CancelInboundOrderCommand request, CancellationToken cancellationToken)
        => _service.CancelAsync(request.Id, request.Reason, cancellationToken);
}
