using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.CostCenters.Queries.GetCostCenterById;

public sealed record GetCostCenterByIdQuery(Guid Id) : IRequest<RequestResult<CostCenterDto>>;
