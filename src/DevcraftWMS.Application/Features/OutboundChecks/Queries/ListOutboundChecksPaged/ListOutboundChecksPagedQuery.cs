using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundChecks.Queries.ListOutboundChecksPaged;

public sealed record ListOutboundChecksPagedQuery(
    Guid? WarehouseId,
    Guid? OutboundOrderId,
    OutboundCheckStatus? Status,
    OutboundOrderPriority? Priority,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir)
    : IRequest<RequestResult<PagedResult<OutboundCheckListItemDto>>>;
