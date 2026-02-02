using MediatR;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Sectors.Queries.ListSectorsPaged;

public sealed class ListSectorsPagedQueryHandler : IRequestHandler<ListSectorsPagedQuery, RequestResult<PagedResult<SectorListItemDto>>>
{
    private readonly ISectorRepository _sectorRepository;

    public ListSectorsPagedQueryHandler(ISectorRepository sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }

    public async Task<RequestResult<PagedResult<SectorListItemDto>>> Handle(ListSectorsPagedQuery request, CancellationToken cancellationToken)
    {
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
            request.PageNumber,
            request.PageSize,
            orderBy,
            orderDir,
            request.Code,
            request.Name,
            request.SectorType,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var dtos = items.Select(SectorMapping.MapListItem).ToList();
        var result = new PagedResult<SectorListItemDto>(dtos, total, request.PageNumber, request.PageSize, orderBy, orderDir);
        return RequestResult<PagedResult<SectorListItemDto>>.Success(result);
    }
}
