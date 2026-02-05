using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Users.Commands.AssignUserRoles;

public sealed record AssignUserRolesCommand(Guid Id, IReadOnlyList<Guid> RoleIds) : IRequest<RequestResult<string>>;
