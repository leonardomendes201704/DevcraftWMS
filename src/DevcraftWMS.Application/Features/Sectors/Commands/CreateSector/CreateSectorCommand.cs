using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Sectors.Commands.CreateSector;

public sealed record CreateSectorCommand(
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description,
    SectorType SectorType) : IRequest<RequestResult<SectorDto>>;
