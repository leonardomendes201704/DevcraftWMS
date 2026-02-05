using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Queries.ListOutboundOrders;

public sealed record ListOutboundOrdersQuery(
    Guid? WarehouseId,
    string? OrderNumber,
    OutboundOrderStatus? Status,
    OutboundOrderPriority? Priority,
    DateTime? CreatedFromUtc,
    DateTime? CreatedToUtc,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir)
    : IRequest<RequestResult<PagedResult<OutboundOrderListItemDto>>>;
