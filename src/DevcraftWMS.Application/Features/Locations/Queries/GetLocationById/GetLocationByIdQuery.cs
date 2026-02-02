using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Locations.Queries.GetLocationById;

public sealed record GetLocationByIdQuery(Guid Id) : MediatR.IRequest<RequestResult<LocationDto>>;
