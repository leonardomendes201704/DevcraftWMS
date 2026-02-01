using MediatR;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Sectors.Queries.ListSectorsPaged;

public sealed class ListSectorsPagedQueryHandler : IRequestHandler<ListSectorsPagedQuery, RequestResult<PagedResult<SectorListItemDto>>>
{
    private const int MaxPageSize = 100;
    private readonly ISectorRepository _sectorRepository;

    public ListSectorsPagedQueryHandler(ISectorRepository sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }

    public async Task<RequestResult<PagedResult<SectorListItemDto>>> Handle(ListSectorsPagedQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 or > MaxPageSize ? 20 : request.PageSize;
        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "CreatedAtUtc" : request.OrderBy;
        var orderDir = string.Equals(request.OrderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        var total = await _sectorRepository.CountAsync(
            request.WarehouseId,
            request.Code,
            request.Name,
            request.SectorType,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _sectorRepository.ListAsync(
            request.WarehouseId,
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            request.Code,
            request.Name,
            request.SectorType,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var dtos = items.Select(SectorMapping.MapListItem).ToList();
        var result = new PagedResult<SectorListItemDto>(dtos, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<SectorListItemDto>>.Success(result);
    }
}
