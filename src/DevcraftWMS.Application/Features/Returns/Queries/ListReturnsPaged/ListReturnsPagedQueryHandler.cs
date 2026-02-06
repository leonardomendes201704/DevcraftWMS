using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.Returns.Queries.ListReturnsPaged;

public sealed class ListReturnsPagedQueryHandler
    : IRequestHandler<ListReturnsPagedQuery, RequestResult<PagedResult<ReturnOrderListItemDto>>>
{
    private readonly IReturnService _service;

    public ListReturnsPagedQueryHandler(IReturnService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<ReturnOrderListItemDto>>> Handle(ListReturnsPagedQuery request, CancellationToken cancellationToken)
        => _service.ListAsync(
            request.WarehouseId,
            request.ReturnNumber,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);
}
