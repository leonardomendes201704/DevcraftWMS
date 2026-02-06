using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.Returns.Queries.ListReturnsPaged;

public sealed record ListReturnsPagedQuery(
    Guid? WarehouseId,
    string? ReturnNumber,
    ReturnStatus? Status,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir)
    : IRequest<RequestResult<PagedResult<ReturnOrderListItemDto>>>;
