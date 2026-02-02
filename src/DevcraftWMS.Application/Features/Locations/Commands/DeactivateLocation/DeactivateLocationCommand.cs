using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Locations.Commands.DeactivateLocation;

public sealed record DeactivateLocationCommand(Guid Id) : MediatR.IRequest<RequestResult<LocationDto>>;
