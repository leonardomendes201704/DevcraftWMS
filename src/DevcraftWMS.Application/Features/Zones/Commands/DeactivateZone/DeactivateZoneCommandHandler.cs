using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Zones.Commands.DeactivateZone;

public sealed class DeactivateZoneCommandHandler : IRequestHandler<DeactivateZoneCommand, RequestResult<ZoneDto>>
{
    private readonly IZoneService _service;

    public DeactivateZoneCommandHandler(IZoneService service)
    {
        _service = service;
    }

    public Task<RequestResult<ZoneDto>> Handle(DeactivateZoneCommand request, CancellationToken cancellationToken)
        => _service.DeactivateZoneAsync(request.Id, cancellationToken);
}
