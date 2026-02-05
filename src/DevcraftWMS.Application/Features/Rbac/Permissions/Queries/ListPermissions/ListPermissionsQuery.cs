using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Queries.ListPermissions;

public sealed record ListPermissionsQuery(
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Search,
    bool? IsActive,
    bool IncludeInactive)
    : IRequest<RequestResult<PagedResult<PermissionDto>>>;
