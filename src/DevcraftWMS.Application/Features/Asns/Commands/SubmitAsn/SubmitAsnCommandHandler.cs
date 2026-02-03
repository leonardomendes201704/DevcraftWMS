using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.SubmitAsn;

public sealed class SubmitAsnCommandHandler : IRequestHandler<SubmitAsnCommand, RequestResult<AsnDetailDto>>
{
    private readonly IAsnService _service;

    public SubmitAsnCommandHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<AsnDetailDto>> Handle(SubmitAsnCommand request, CancellationToken cancellationToken)
        => _service.SubmitAsync(request.AsnId, request.Notes, cancellationToken);
}
