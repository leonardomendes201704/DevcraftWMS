using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.Zones.Queries.ListZonesPaged;

public sealed class ListZonesPagedQueryHandler : IRequestHandler<ListZonesPagedQuery, RequestResult<PagedResult<ZoneListItemDto>>>
{
    private readonly IZoneRepository _zoneRepository;

    public ListZonesPagedQueryHandler(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public async Task<RequestResult<PagedResult<ZoneListItemDto>>> Handle(ListZonesPagedQuery request, CancellationToken cancellationToken)
    {
        var total = await _zoneRepository.CountAsync(
            request.WarehouseId,
            request.Code,
            request.Name,
            request.ZoneType,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _zoneRepository.ListAsync(
            request.WarehouseId,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            request.Code,
            request.Name,
            request.ZoneType,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var mapped = items.Select(ZoneMapping.MapListItem).ToList();
        var result = new PagedResult<ZoneListItemDto>(mapped, total, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<ZoneListItemDto>>.Success(result);
    }
}
