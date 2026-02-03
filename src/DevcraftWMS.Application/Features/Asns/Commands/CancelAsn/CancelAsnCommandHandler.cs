using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.CancelAsn;

public sealed class CancelAsnCommandHandler : IRequestHandler<CancelAsnCommand, RequestResult<AsnDetailDto>>
{
    private readonly IAsnService _service;

    public CancelAsnCommandHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<AsnDetailDto>> Handle(CancelAsnCommand request, CancellationToken cancellationToken)
        => _service.CancelAsync(request.AsnId, request.Notes, cancellationToken);
}
