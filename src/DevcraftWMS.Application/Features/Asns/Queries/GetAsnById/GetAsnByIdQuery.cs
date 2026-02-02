using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Queries.GetAsnById;

public sealed record GetAsnByIdQuery(Guid Id) : IRequest<RequestResult<AsnDetailDto>>;
