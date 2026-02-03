using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.UnitLoads.Queries.ListUnitLoadsPaged;

public sealed record ListUnitLoadsPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    Guid? WarehouseId = null,
    Guid? ReceiptId = null,
    string? Sscc = null,
    UnitLoadStatus? Status = null,
    bool? IsActive = null,
    bool IncludeInactive = false) : IRequest<RequestResult<PagedResult<UnitLoadListItemDto>>>;
