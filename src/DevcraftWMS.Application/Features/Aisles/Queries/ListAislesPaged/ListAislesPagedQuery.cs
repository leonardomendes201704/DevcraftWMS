using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.Aisles.Queries.ListAislesPaged;

public sealed record ListAislesPagedQuery(
    Guid SectionId,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Code,
    string? Name,
    bool? IsActive,
    bool IncludeInactive) : IRequest<RequestResult<PagedResult<AisleListItemDto>>>;
