using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.Lots.Queries.ListLotsPaged;

public sealed record ListLotsPagedQuery(
    Guid ProductId,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Code = null,
    LotStatus? Status = null,
    DateOnly? ExpirationFrom = null,
    DateOnly? ExpirationTo = null,
    bool? IsActive = null,
    bool IncludeInactive = false) : IRequest<RequestResult<PagedResult<LotListItemDto>>>;
