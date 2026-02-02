using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Locations.Commands.DeactivateLocation;

public sealed class DeactivateLocationCommandHandler : MediatR.IRequestHandler<DeactivateLocationCommand, RequestResult<LocationDto>>
{
    private readonly ILocationService _locationService;

    public DeactivateLocationCommandHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public Task<RequestResult<LocationDto>> Handle(DeactivateLocationCommand request, CancellationToken cancellationToken)
        => _locationService.DeactivateLocationAsync(request.Id, cancellationToken);
}
