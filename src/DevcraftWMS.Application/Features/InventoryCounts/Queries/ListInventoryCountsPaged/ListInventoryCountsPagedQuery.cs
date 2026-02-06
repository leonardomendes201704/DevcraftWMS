using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryCounts.Queries.ListInventoryCountsPaged;

public sealed record ListInventoryCountsPagedQuery(
    Guid? WarehouseId,
    Guid? LocationId,
    InventoryCountStatus? Status,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir)
    : IRequest<RequestResult<PagedResult<InventoryCountListItemDto>>>;
