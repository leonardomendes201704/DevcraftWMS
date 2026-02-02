using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Locations.Queries.ListLocationsPaged;

public sealed class ListLocationsPagedQueryHandler : MediatR.IRequestHandler<ListLocationsPagedQuery, RequestResult<PagedResult<LocationListItemDto>>>
{
    private readonly ILocationRepository _locationRepository;

    public ListLocationsPagedQueryHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<RequestResult<PagedResult<LocationListItemDto>>> Handle(ListLocationsPagedQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _locationRepository.CountAsync(
            request.StructureId,
            request.Code,
            request.Barcode,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _locationRepository.ListAsync(
            request.StructureId,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            request.Code,
            request.Barcode,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var mapped = items.Select(LocationMapping.MapListItem).ToList();
        var result = new PagedResult<LocationListItemDto>(mapped, totalCount, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<LocationListItemDto>>.Success(result);
    }
}
