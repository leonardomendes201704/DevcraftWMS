using MediatR;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Warehouses.Queries.ListWarehousesPaged;

public sealed class ListWarehousesPagedQueryHandler : IRequestHandler<ListWarehousesPagedQuery, RequestResult<PagedResult<WarehouseListItemDto>>>
{
    private const int MaxPageSize = 100;
    private readonly IWarehouseRepository _warehouseRepository;

    public ListWarehousesPagedQueryHandler(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<RequestResult<PagedResult<WarehouseListItemDto>>> Handle(ListWarehousesPagedQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 or > MaxPageSize ? 20 : request.PageSize;
        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "CreatedAtUtc" : request.OrderBy;
        var orderDir = string.Equals(request.OrderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        var total = await _warehouseRepository.CountAsync(
            request.Search,
            request.Code,
            request.Name,
            request.WarehouseType,
            request.City,
            request.State,
            request.Country,
            request.ExternalId,
            request.ErpCode,
            request.CostCenterCode,
            request.IsPrimary,
            request.IncludeInactive,
            cancellationToken);

        var items = await _warehouseRepository.ListAsync(
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            request.Search,
            request.Code,
            request.Name,
            request.WarehouseType,
            request.City,
            request.State,
            request.Country,
            request.ExternalId,
            request.ErpCode,
            request.CostCenterCode,
            request.IsPrimary,
            request.IncludeInactive,
            cancellationToken);

        var dtos = items.Select(WarehouseMapping.MapListItem).ToList();
        var result = new PagedResult<WarehouseListItemDto>(dtos, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<WarehouseListItemDto>>.Success(result);
    }
}
