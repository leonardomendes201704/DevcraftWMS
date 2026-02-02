using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Zones.Commands.UpdateZone;

public sealed class UpdateZoneCommandHandler : IRequestHandler<UpdateZoneCommand, RequestResult<ZoneDto>>
{
    private readonly IZoneService _service;

    public UpdateZoneCommandHandler(IZoneService service)
    {
        _service = service;
    }

    public Task<RequestResult<ZoneDto>> Handle(UpdateZoneCommand request, CancellationToken cancellationToken)
        => _service.UpdateZoneAsync(
            request.Id,
            request.WarehouseId,
            request.Code,
            request.Name,
            request.Description,
            request.ZoneType,
            cancellationToken);
}
