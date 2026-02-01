using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Warehouses.Commands.DeactivateWarehouse;

public sealed class DeactivateWarehouseCommandHandler : IRequestHandler<DeactivateWarehouseCommand, RequestResult<WarehouseDto>>
{
    private readonly IWarehouseService _warehouseService;

    public DeactivateWarehouseCommandHandler(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    public async Task<RequestResult<WarehouseDto>> Handle(DeactivateWarehouseCommand request, CancellationToken cancellationToken)
    {
        return await _warehouseService.DeactivateWarehouseAsync(request.Id, cancellationToken);
    }
}
