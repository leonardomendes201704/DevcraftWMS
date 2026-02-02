using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Uoms.Queries.GetUomById;

public sealed record GetUomByIdQuery(Guid Id) : IRequest<RequestResult<UomDto>>;
