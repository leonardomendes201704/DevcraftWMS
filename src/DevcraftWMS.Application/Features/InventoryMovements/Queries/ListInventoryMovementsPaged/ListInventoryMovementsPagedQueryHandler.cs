using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.InventoryMovements.Queries.ListInventoryMovementsPaged;

public sealed class ListInventoryMovementsPagedQueryHandler
    : IRequestHandler<ListInventoryMovementsPagedQuery, RequestResult<PagedResult<InventoryMovementListItemDto>>>
{
    private readonly IInventoryMovementService _service;

    public ListInventoryMovementsPagedQueryHandler(IInventoryMovementService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<InventoryMovementListItemDto>>> Handle(ListInventoryMovementsPagedQuery request, CancellationToken cancellationToken)
    {
        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "PerformedAtUtc" : request.OrderBy;
        var orderDir = string.Equals(request.OrderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        return _service.ListAsync(
            request.ProductId,
            request.FromLocationId,
            request.ToLocationId,
            request.LotId,
            request.Status,
            request.PerformedFromUtc,
            request.PerformedToUtc,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            orderBy,
            orderDir,
            cancellationToken);
    }
}
