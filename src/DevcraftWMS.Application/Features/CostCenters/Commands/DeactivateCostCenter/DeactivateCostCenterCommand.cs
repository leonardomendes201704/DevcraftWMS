using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.CostCenters.Commands.DeactivateCostCenter;

public sealed record DeactivateCostCenterCommand(Guid Id) : IRequest<RequestResult<CostCenterDto>>;
