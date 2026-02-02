using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Uoms.Commands.DeactivateUom;

public sealed class DeactivateUomCommandHandler : IRequestHandler<DeactivateUomCommand, RequestResult<UomDto>>
{
    private readonly IUomService _service;

    public DeactivateUomCommandHandler(IUomService service)
    {
        _service = service;
    }

    public Task<RequestResult<UomDto>> Handle(DeactivateUomCommand request, CancellationToken cancellationToken)
        => _service.DeactivateUomAsync(request.Id, cancellationToken);
}
