using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.UnitLoads.Queries.GetUnitLoadById;

public sealed record GetUnitLoadByIdQuery(Guid Id) : IRequest<RequestResult<UnitLoadDetailDto>>;
