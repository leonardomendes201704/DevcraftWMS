using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.CompleteInboundOrder;

public sealed class CompleteInboundOrderCommandHandler : IRequestHandler<CompleteInboundOrderCommand, RequestResult<InboundOrderDetailDto>>
{
    private readonly IInboundOrderService _service;

    public CompleteInboundOrderCommandHandler(IInboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<InboundOrderDetailDto>> Handle(CompleteInboundOrderCommand request, CancellationToken cancellationToken)
        => _service.CompleteAsync(request.Id, request.AllowPartial, request.Notes, cancellationToken);
}
