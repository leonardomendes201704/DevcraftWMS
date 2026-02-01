using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Warehouses.Queries.ListWarehousesPaged;

public sealed record ListWarehousesPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Search = null,
    string? Code = null,
    string? Name = null,
    WarehouseType? WarehouseType = null,
    string? City = null,
    string? State = null,
    string? Country = null,
    string? ExternalId = null,
    string? ErpCode = null,
    string? CostCenterCode = null,
    bool? IsPrimary = null,
    bool IncludeInactive = false) : IRequest<RequestResult<PagedResult<WarehouseListItemDto>>>;
