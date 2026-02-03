using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.ListReceiptDivergences;

public sealed class ListReceiptDivergencesQueryHandler : IRequestHandler<ListReceiptDivergencesQuery, RequestResult<IReadOnlyList<ReceiptDivergenceDto>>>
{
    private readonly IReceiptDivergenceService _service;

    public ListReceiptDivergencesQueryHandler(IReceiptDivergenceService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<ReceiptDivergenceDto>>> Handle(ListReceiptDivergencesQuery request, CancellationToken cancellationToken)
        => _service.ListByReceiptAsync(request.ReceiptId, cancellationToken);
}
