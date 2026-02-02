using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryBalances.Queries.ListInventoryBalances;

public sealed record ListInventoryBalancesQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    Guid? LocationId = null,
    Guid? ProductId = null,
    Guid? LotId = null,
    InventoryBalanceStatus? Status = null,
    bool? IsActive = null,
    bool IncludeInactive = false) : IRequest<RequestResult<PagedResult<InventoryBalanceListItemDto>>>;
