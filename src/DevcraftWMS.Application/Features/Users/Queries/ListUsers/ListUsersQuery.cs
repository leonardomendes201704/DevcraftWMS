using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Users.Queries.ListUsers;

public sealed record ListUsersQuery(
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Search,
    bool? IsActive,
    bool IncludeInactive)
    : IRequest<RequestResult<PagedResult<UserListItemDto>>>;
