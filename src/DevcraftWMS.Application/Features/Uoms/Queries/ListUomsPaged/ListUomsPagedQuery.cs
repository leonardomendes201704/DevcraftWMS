using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.Uoms.Queries.ListUomsPaged;

public sealed record ListUomsPagedQuery(
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Code,
    string? Name,
    UomType? Type,
    bool? IsActive,
    bool IncludeInactive) : IRequest<RequestResult<PagedResult<UomListItemDto>>>;
