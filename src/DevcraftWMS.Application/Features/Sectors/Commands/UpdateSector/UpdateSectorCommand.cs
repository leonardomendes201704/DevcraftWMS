using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Sectors.Commands.UpdateSector;

public sealed record UpdateSectorCommand(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description,
    SectorType SectorType) : IRequest<RequestResult<SectorDto>>;
