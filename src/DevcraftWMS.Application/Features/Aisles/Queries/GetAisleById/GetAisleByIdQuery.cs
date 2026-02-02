using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Aisles.Queries.GetAisleById;

public sealed record GetAisleByIdQuery(Guid Id) : IRequest<RequestResult<AisleDto>>;
