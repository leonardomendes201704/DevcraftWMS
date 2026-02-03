using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.ListInboundOrderDivergences;

public sealed class ListInboundOrderDivergencesQueryHandler : IRequestHandler<ListInboundOrderDivergencesQuery, RequestResult<IReadOnlyList<ReceiptDivergenceDto>>>
{
    private readonly IReceiptDivergenceService _service;

    public ListInboundOrderDivergencesQueryHandler(IReceiptDivergenceService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<ReceiptDivergenceDto>>> Handle(ListInboundOrderDivergencesQuery request, CancellationToken cancellationToken)
        => _service.ListByInboundOrderAsync(request.InboundOrderId, cancellationToken);
}
