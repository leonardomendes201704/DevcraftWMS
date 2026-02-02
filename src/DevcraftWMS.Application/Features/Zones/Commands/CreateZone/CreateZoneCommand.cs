using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.Zones.Commands.CreateZone;

public sealed record CreateZoneCommand(
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description,
    ZoneType ZoneType) : IRequest<RequestResult<ZoneDto>>;
