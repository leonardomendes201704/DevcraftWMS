using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.CostCenters.Commands.CreateCostCenter;

public sealed class CreateCostCenterCommandHandler : IRequestHandler<CreateCostCenterCommand, RequestResult<CostCenterDto>>
{
    private readonly ICostCenterService _service;

    public CreateCostCenterCommandHandler(ICostCenterService service)
    {
        _service = service;
    }

    public Task<RequestResult<CostCenterDto>> Handle(CreateCostCenterCommand request, CancellationToken cancellationToken)
        => _service.CreateCostCenterAsync(request.Code, request.Name, request.Description, cancellationToken);
}
