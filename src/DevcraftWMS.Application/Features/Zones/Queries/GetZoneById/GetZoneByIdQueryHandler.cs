using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Zones.Queries.GetZoneById;

public sealed class GetZoneByIdQueryHandler : IRequestHandler<GetZoneByIdQuery, RequestResult<ZoneDto>>
{
    private readonly IZoneRepository _zoneRepository;

    public GetZoneByIdQueryHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public async Task<RequestResult<ZoneDto>> Handle(GetZoneByIdQuery request, CancellationToken cancellationToken)
    {
        var zone = await _zoneRepository.GetByIdAsync(request.Id, cancellationToken);
        return zone is null
            ? RequestResult<ZoneDto>.Failure("zones.zone.not_found", "Zone not found.")
            : RequestResult<ZoneDto>.Success(ZoneMapping.Map(zone));
    }
}
