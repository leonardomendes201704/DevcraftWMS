using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Sections.Queries.ListSectionsPaged;

public sealed record ListSectionsPagedQuery(
    Guid SectorId,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Code,
    string? Name,
    bool? IsActive,
    bool IncludeInactive) : MediatR.IRequest<RequestResult<PagedResult<SectionListItemDto>>>;
