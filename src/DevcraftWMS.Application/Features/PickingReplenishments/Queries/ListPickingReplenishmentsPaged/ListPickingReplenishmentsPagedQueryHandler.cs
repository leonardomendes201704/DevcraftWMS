using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.PickingReplenishments.Queries.ListPickingReplenishmentsPaged;

public sealed class ListPickingReplenishmentsPagedQueryHandler
    : IRequestHandler<ListPickingReplenishmentsPagedQuery, RequestResult<PagedResult<PickingReplenishmentListItemDto>>>
{
    private readonly PickingReplenishmentService _service;

    public ListPickingReplenishmentsPagedQueryHandler(PickingReplenishmentService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<PickingReplenishmentListItemDto>>> Handle(
        ListPickingReplenishmentsPagedQuery request,
        CancellationToken cancellationToken)
        => _service.ListAsync(
            request.WarehouseId,
            request.ProductId,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);
}
