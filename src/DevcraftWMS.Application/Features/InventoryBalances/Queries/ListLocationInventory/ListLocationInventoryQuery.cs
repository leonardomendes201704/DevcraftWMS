using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryBalances.Queries.ListLocationInventory;

public sealed record ListLocationInventoryQuery(
    Guid LocationId,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    Guid? ProductId = null,
    Guid? LotId = null,
    InventoryBalanceStatus? Status = null,
    bool? IsActive = null,
    bool IncludeInactive = false) : IRequest<RequestResult<PagedResult<InventoryBalanceListItemDto>>>;
