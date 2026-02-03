using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.GetReceiptDivergenceEvidence;

public sealed class GetReceiptDivergenceEvidenceQueryHandler : IRequestHandler<GetReceiptDivergenceEvidenceQuery, RequestResult<ReceiptDivergenceEvidenceFileDto>>
{
    private readonly IReceiptDivergenceService _service;

    public GetReceiptDivergenceEvidenceQueryHandler(IReceiptDivergenceService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReceiptDivergenceEvidenceFileDto>> Handle(GetReceiptDivergenceEvidenceQuery request, CancellationToken cancellationToken)
        => _service.GetEvidenceAsync(request.DivergenceId, request.EvidenceId, cancellationToken);
}
