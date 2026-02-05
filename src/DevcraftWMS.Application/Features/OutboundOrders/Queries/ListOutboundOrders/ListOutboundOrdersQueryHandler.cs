using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Queries.ListOutboundOrders;

public sealed class ListOutboundOrdersQueryHandler
    : IRequestHandler<ListOutboundOrdersQuery, RequestResult<PagedResult<OutboundOrderListItemDto>>>
{
    private readonly IOutboundOrderService _service;

    public ListOutboundOrdersQueryHandler(IOutboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<OutboundOrderListItemDto>>> Handle(
        ListOutboundOrdersQuery request,
        CancellationToken cancellationToken)
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
