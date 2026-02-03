using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.GateCheckins.Commands.AssignDock;

public sealed class AssignGateDockCommandHandler : IRequestHandler<AssignGateDockCommand, RequestResult<GateCheckinDetailDto>>
{
    private readonly IGateCheckinService _service;

    public AssignGateDockCommandHandler(IGateCheckinService service)
    {
        _service = service;
    }

    public Task<RequestResult<GateCheckinDetailDto>> Handle(AssignGateDockCommand request, CancellationToken cancellationToken)
        => _service.AssignDockAsync(request.Id, request.DockCode, cancellationToken);
}
