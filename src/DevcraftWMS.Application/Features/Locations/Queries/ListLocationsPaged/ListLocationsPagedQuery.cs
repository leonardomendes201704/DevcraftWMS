using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Locations.Queries.ListLocationsPaged;

public sealed record ListLocationsPagedQuery(
    Guid StructureId,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Code,
    string? Barcode,
    bool? IsActive,
    bool IncludeInactive) : MediatR.IRequest<RequestResult<PagedResult<LocationListItemDto>>>;
