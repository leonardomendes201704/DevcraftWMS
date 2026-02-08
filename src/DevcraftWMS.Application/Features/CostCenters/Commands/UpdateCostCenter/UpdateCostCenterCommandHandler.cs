using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.CostCenters.Commands.UpdateCostCenter;

public sealed class UpdateCostCenterCommandHandler : IRequestHandler<UpdateCostCenterCommand, RequestResult<CostCenterDto>>
{
    private readonly ICostCenterService _service;

    public UpdateCostCenterCommandHandler(ICostCenterService service)
    {
        _service = service;
    }

    public Task<RequestResult<CostCenterDto>> Handle(UpdateCostCenterCommand request, CancellationToken cancellationToken)
        => _service.UpdateCostCenterAsync(request.Id, request.Code, request.Name, request.Description, cancellationToken);
}
