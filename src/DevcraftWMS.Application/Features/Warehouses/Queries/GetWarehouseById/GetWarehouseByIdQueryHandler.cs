using MediatR;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Warehouses.Queries.GetWarehouseById;

public sealed class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, RequestResult<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;

    public GetWarehouseByIdQueryHandler(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<RequestResult<WarehouseDto>> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<WarehouseDto>.Failure("warehouses.warehouse.not_found", "Warehouse not found.");
        }

        var dto = WarehouseMapping.MapWarehouse(warehouse);
        return RequestResult<WarehouseDto>.Success(dto);
    }
}
