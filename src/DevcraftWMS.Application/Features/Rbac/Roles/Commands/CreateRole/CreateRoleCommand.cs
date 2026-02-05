using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Commands.CreateRole;

public sealed record CreateRoleCommand(
    string Name,
    string? Description,
    IReadOnlyList<Guid> PermissionIds)
    : IRequest<RequestResult<RoleDetailDto>>;
