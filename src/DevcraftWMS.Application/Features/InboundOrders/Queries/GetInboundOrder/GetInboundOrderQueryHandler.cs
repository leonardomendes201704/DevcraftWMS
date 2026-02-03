using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Queries.GetInboundOrder;

public sealed class GetInboundOrderQueryHandler : IRequestHandler<GetInboundOrderQuery, RequestResult<InboundOrderDetailDto>>
{
    private readonly IInboundOrderService _service;

    public GetInboundOrderQueryHandler(IInboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<InboundOrderDetailDto>> Handle(GetInboundOrderQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.Id, cancellationToken);
}
