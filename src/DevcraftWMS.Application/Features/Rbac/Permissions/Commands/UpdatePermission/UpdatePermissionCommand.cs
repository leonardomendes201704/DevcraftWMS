using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Commands.UpdatePermission;

public sealed record UpdatePermissionCommand(
    Guid Id,
    string Code,
    string Name,
    string? Description)
    : IRequest<RequestResult<PermissionDto>>;
