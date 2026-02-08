using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Structures.Queries.ListStructuresForCustomerPaged;

public sealed record ListStructuresForCustomerPagedQuery(
    Guid? WarehouseId = null,
    Guid? SectorId = null,
    Guid? SectionId = null,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Code = null,
    string? Name = null,
    StructureType? StructureType = null,
    bool? IsActive = null,
    bool IncludeInactive = false)
    : MediatR.IRequest<RequestResult<PagedResult<StructureListItemDto>>>;
