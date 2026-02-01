using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sectors.Commands.DeactivateSector;

public sealed record DeactivateSectorCommand(Guid Id) : IRequest<RequestResult<SectorDto>>;
