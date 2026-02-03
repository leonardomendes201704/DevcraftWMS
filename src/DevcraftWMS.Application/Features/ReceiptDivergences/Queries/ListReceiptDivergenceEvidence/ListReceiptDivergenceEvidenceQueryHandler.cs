using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.ListReceiptDivergenceEvidence;

public sealed class ListReceiptDivergenceEvidenceQueryHandler : IRequestHandler<ListReceiptDivergenceEvidenceQuery, RequestResult<IReadOnlyList<ReceiptDivergenceEvidenceDto>>>
{
    private readonly IReceiptDivergenceService _service;

    public ListReceiptDivergenceEvidenceQueryHandler(IReceiptDivergenceService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<ReceiptDivergenceEvidenceDto>>> Handle(ListReceiptDivergenceEvidenceQuery request, CancellationToken cancellationToken)
        => _service.ListEvidenceAsync(request.DivergenceId, cancellationToken);
}
