using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.CostCenters.Commands.CreateCostCenter;

public sealed record CreateCostCenterCommand(
    string Code,
    string Name,
    string? Description) : IRequest<RequestResult<CostCenterDto>>;
