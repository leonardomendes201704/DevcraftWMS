using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.CostCenters.Queries.ListCostCentersPaged;

public sealed record ListCostCentersPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Code = null,
    string? Name = null,
    bool? IsActive = null,
    bool IncludeInactive = false) : IRequest<RequestResult<PagedResult<CostCenterListItemDto>>>;
