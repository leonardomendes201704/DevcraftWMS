using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Locations.Commands.CreateLocation;

public sealed class CreateLocationCommandHandler : MediatR.IRequestHandler<CreateLocationCommand, RequestResult<LocationDto>>
{
    private readonly ILocationService _locationService;

    public CreateLocationCommandHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public Task<RequestResult<LocationDto>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        => _locationService.CreateLocationAsync(request.StructureId, request.Code, request.Barcode, request.Level, request.Row, request.Column, cancellationToken);
}
