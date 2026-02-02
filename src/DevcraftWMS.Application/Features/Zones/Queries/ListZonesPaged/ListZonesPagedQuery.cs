using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Zones.Queries.ListZonesPaged;

public sealed record ListZonesPagedQuery(
    Guid WarehouseId,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Code,
    string? Name,
    ZoneType? ZoneType,
    bool? IsActive,
    bool IncludeInactive) : MediatR.IRequest<RequestResult<PagedResult<ZoneListItemDto>>>;
