using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Queries.ListPermissions;

public sealed class ListPermissionsQueryHandler : IRequestHandler<ListPermissionsQuery, RequestResult<PagedResult<PermissionDto>>>
{
    private readonly PermissionService _service;

    public ListPermissionsQueryHandler(PermissionService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<PermissionDto>>> Handle(ListPermissionsQuery request, CancellationToken cancellationToken)
        => _service.ListAsync(request.Search, request.IsActive, request.IncludeInactive, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir, cancellationToken);
}
