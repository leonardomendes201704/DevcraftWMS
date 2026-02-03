using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.GateCheckins.Commands.DeactivateGateCheckin;

public sealed class DeactivateGateCheckinCommandHandler : IRequestHandler<DeactivateGateCheckinCommand, RequestResult<GateCheckinDetailDto>>
{
    private readonly IGateCheckinService _service;

    public DeactivateGateCheckinCommandHandler(IGateCheckinService service)
    {
        _service = service;
    }

    public Task<RequestResult<GateCheckinDetailDto>> Handle(DeactivateGateCheckinCommand request, CancellationToken cancellationToken)
        => _service.DeactivateAsync(request.Id, cancellationToken);
}
