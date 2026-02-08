using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.CostCenters.Commands.UpdateCostCenter;

public sealed record UpdateCostCenterCommand(
    Guid Id,
    string Code,
    string Name,
    string? Description) : IRequest<RequestResult<CostCenterDto>>;
