using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Zones.Commands.CreateZone;

public sealed class CreateZoneCommandHandler : IRequestHandler<CreateZoneCommand, RequestResult<ZoneDto>>
{
    private readonly IZoneService _service;

    public CreateZoneCommandHandler(IZoneService service)
    {
        _service = service;
    }

    public Task<RequestResult<ZoneDto>> Handle(CreateZoneCommand request, CancellationToken cancellationToken)
        => _service.CreateZoneAsync(
            request.WarehouseId,
            request.Code,
            request.Name,
            request.Description,
            request.ZoneType,
            cancellationToken);
}
