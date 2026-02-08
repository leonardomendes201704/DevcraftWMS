using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.CostCenters.Commands.DeactivateCostCenter;

public sealed class DeactivateCostCenterCommandHandler : IRequestHandler<DeactivateCostCenterCommand, RequestResult<CostCenterDto>>
{
    private readonly ICostCenterService _service;

    public DeactivateCostCenterCommandHandler(ICostCenterService service)
    {
        _service = service;
    }

    public Task<RequestResult<CostCenterDto>> Handle(DeactivateCostCenterCommand request, CancellationToken cancellationToken)
        => _service.DeactivateCostCenterAsync(request.Id, cancellationToken);
}
