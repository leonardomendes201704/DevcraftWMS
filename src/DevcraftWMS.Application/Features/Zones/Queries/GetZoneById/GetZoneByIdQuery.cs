using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Zones.Queries.GetZoneById;

public sealed record GetZoneByIdQuery(Guid Id) : IRequest<RequestResult<ZoneDto>>;
