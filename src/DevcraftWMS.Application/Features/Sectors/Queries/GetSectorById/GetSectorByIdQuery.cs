using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sectors.Queries.GetSectorById;

public sealed record GetSectorByIdQuery(Guid Id) : IRequest<RequestResult<SectorDto>>;
