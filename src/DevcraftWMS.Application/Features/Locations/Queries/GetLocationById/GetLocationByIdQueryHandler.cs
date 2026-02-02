using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Locations.Queries.GetLocationById;

public sealed class GetLocationByIdQueryHandler : MediatR.IRequestHandler<GetLocationByIdQuery, RequestResult<LocationDto>>
{
    private readonly ILocationRepository _locationRepository;

    public GetLocationByIdQueryHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<RequestResult<LocationDto>> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (location is null)
        {
            return RequestResult<LocationDto>.Failure("locations.location.not_found", "Location not found.");
        }

        return RequestResult<LocationDto>.Success(LocationMapping.Map(location));
    }
}
