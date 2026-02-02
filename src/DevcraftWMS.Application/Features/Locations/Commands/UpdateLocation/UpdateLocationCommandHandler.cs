using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Locations.Commands.UpdateLocation;

public sealed class UpdateLocationCommandHandler : MediatR.IRequestHandler<UpdateLocationCommand, RequestResult<LocationDto>>
{
    private readonly ILocationService _locationService;

    public UpdateLocationCommandHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public Task<RequestResult<LocationDto>> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        => _locationService.UpdateLocationAsync(
            request.Id,
            request.StructureId,
            request.ZoneId,
            request.Code,
            request.Barcode,
            request.Level,
            request.Row,
            request.Column,
            request.MaxWeightKg,
            request.MaxVolumeM3,
            request.AllowLotTracking,
            request.AllowExpiryTracking,
            cancellationToken);
}
