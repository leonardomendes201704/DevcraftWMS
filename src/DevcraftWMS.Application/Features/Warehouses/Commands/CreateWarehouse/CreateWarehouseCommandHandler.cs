using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Warehouses.Commands.CreateWarehouse;

public sealed class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, RequestResult<WarehouseDto>>
{
    private readonly IWarehouseService _warehouseService;

    public CreateWarehouseCommandHandler(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    public async Task<RequestResult<WarehouseDto>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        return await _warehouseService.CreateWarehouseAsync(
            request.Code,
            request.Name,
            request.ShortName,
            request.Description,
            request.WarehouseType,
            request.IsPrimary,
            request.IsPickingEnabled,
            request.IsReceivingEnabled,
            request.IsShippingEnabled,
            request.IsReturnsEnabled,
            request.ExternalId,
            request.ErpCode,
            request.CostCenterCode,
            request.CostCenterName,
            request.CutoffTime,
            request.Timezone,
            request.Address,
            request.Contact,
            request.Capacity,
            cancellationToken);
    }
}
