using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Queries.ListInboundOrders;

public sealed class ListInboundOrdersQueryHandler : IRequestHandler<ListInboundOrdersQuery, RequestResult<PagedResult<InboundOrderListItemDto>>>
{
    private readonly IInboundOrderService _service;

    public ListInboundOrdersQueryHandler(IInboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<InboundOrderListItemDto>>> Handle(ListInboundOrdersQuery request, CancellationToken cancellationToken)
        => _service.ListAsync(
            request.WarehouseId,
            request.OrderNumber,
            request.Status,
            request.Priority,
            request.CreatedFromUtc,
            request.CreatedToUtc,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);
}
