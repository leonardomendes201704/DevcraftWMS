using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Queries.ListInboundOrders;

public sealed record ListInboundOrdersQuery(
    Guid? WarehouseId,
    string? OrderNumber,
    InboundOrderStatus? Status,
    InboundOrderPriority? Priority,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir) : IRequest<RequestResult<PagedResult<InboundOrderListItemDto>>>;
