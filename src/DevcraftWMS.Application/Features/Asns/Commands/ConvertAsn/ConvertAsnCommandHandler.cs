using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.ConvertAsn;

public sealed class ConvertAsnCommandHandler : IRequestHandler<ConvertAsnCommand, RequestResult<AsnDetailDto>>
{
    private readonly IAsnService _service;

    public ConvertAsnCommandHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<AsnDetailDto>> Handle(ConvertAsnCommand request, CancellationToken cancellationToken)
        => _service.ConvertAsync(request.AsnId, request.Notes, cancellationToken);
}
