using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.ApproveAsn;

public sealed class ApproveAsnCommandHandler : IRequestHandler<ApproveAsnCommand, RequestResult<AsnDetailDto>>
{
    private readonly IAsnService _service;

    public ApproveAsnCommandHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<AsnDetailDto>> Handle(ApproveAsnCommand request, CancellationToken cancellationToken)
        => _service.ApproveAsync(request.AsnId, request.Notes, cancellationToken);
}
