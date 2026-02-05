using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string FullName,
    string Password,
    IReadOnlyList<Guid> RoleIds)
    : IRequest<RequestResult<UserDetailDto>>;
