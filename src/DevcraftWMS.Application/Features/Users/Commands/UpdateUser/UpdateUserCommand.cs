using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string Email,
    string FullName,
    IReadOnlyList<Guid> RoleIds)
    : IRequest<RequestResult<UserDetailDto>>;
