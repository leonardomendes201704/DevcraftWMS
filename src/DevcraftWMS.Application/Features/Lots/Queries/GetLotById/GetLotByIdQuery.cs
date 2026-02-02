using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Lots.Queries.GetLotById;

public sealed record GetLotByIdQuery(Guid Id) : IRequest<RequestResult<LotDto>>;
