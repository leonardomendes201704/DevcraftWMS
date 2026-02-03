using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.ConvertAsnToInboundOrder;

public sealed class ConvertAsnToInboundOrderCommandHandler : IRequestHandler<ConvertAsnToInboundOrderCommand, RequestResult<InboundOrderDetailDto>>
{
    private readonly IInboundOrderService _service;

    public ConvertAsnToInboundOrderCommandHandler(IInboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<InboundOrderDetailDto>> Handle(ConvertAsnToInboundOrderCommand request, CancellationToken cancellationToken)
        => _service.ConvertFromAsnAsync(request.AsnId, request.Notes, cancellationToken);
}
