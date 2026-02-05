using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Commands.UpdateRole;

public sealed record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<Guid> PermissionIds)
    : IRequest<RequestResult<RoleDetailDto>>;
