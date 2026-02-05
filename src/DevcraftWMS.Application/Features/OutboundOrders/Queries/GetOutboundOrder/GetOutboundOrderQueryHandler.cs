using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Queries.GetOutboundOrder;

public sealed class GetOutboundOrderQueryHandler
    : IRequestHandler<GetOutboundOrderQuery, RequestResult<OutboundOrderDetailDto>>
{
    private readonly IOutboundOrderService _service;

    public GetOutboundOrderQueryHandler(IOutboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<OutboundOrderDetailDto>> Handle(
        GetOutboundOrderQuery request,
        CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.Id, cancellationToken);
}
