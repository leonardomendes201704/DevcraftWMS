using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Commands.AddReceiptDivergenceEvidence;

public sealed class AddReceiptDivergenceEvidenceCommandHandler : IRequestHandler<AddReceiptDivergenceEvidenceCommand, RequestResult<ReceiptDivergenceEvidenceDto>>
{
    private readonly IReceiptDivergenceService _service;

    public AddReceiptDivergenceEvidenceCommandHandler(IReceiptDivergenceService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReceiptDivergenceEvidenceDto>> Handle(AddReceiptDivergenceEvidenceCommand request, CancellationToken cancellationToken)
        => _service.AddEvidenceAsync(request.DivergenceId, request.Evidence, cancellationToken);
}
