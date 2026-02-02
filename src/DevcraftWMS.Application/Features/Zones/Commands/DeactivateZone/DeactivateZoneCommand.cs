using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Zones.Commands.DeactivateZone;

public sealed record DeactivateZoneCommand(Guid Id) : IRequest<RequestResult<ZoneDto>>;
