using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Queries.ListRoles;

public sealed record ListRolesQuery(
    string? Search,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir) : IRequest<RequestResult<PagedResult<RoleListItemDto>>>;
