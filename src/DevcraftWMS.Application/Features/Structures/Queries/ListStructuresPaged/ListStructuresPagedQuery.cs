using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Structures.Queries.ListStructuresPaged;

public sealed record ListStructuresPagedQuery(
    Guid SectionId,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Code,
    string? Name,
    StructureType? StructureType,
    bool? IsActive,
    bool IncludeInactive) : MediatR.IRequest<RequestResult<PagedResult<StructureListItemDto>>>;
