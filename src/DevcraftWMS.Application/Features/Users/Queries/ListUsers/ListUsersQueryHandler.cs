using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Users.Queries.ListUsers;

public sealed class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, RequestResult<PagedResult<UserListItemDto>>>
{
    private readonly UserManagementService _service;

    public ListUsersQueryHandler(UserManagementService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<UserListItemDto>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        => _service.ListAsync(request.Search, request.IsActive, request.IncludeInactive, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir, cancellationToken);
}
