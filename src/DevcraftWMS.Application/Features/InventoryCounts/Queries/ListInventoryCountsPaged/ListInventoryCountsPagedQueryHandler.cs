using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryCounts.Queries.ListInventoryCountsPaged;

public sealed class ListInventoryCountsPagedQueryHandler
    : IRequestHandler<ListInventoryCountsPagedQuery, RequestResult<PagedResult<InventoryCountListItemDto>>>
{
    private readonly IInventoryCountService _service;

    public ListInventoryCountsPagedQueryHandler(IInventoryCountService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<InventoryCountListItemDto>>> Handle(ListInventoryCountsPagedQuery request, CancellationToken cancellationToken)
        => _service.ListAsync(
            request.WarehouseId,
            request.LocationId,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);
}
