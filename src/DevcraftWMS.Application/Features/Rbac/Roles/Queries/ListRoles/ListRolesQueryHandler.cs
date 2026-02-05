using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Queries.ListRoles;

public sealed class ListRolesQueryHandler : IRequestHandler<ListRolesQuery, RequestResult<PagedResult<RoleListItemDto>>>
{
    private readonly RoleService _service;

    public ListRolesQueryHandler(RoleService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<RoleListItemDto>>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
        => _service.ListAsync(request.Search, request.IsActive, request.IncludeInactive, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir, cancellationToken);
}
