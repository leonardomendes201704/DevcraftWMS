using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Commands.CreatePermission;

public sealed record CreatePermissionCommand(
    string Code,
    string Name,
    string? Description)
    : IRequest<RequestResult<PermissionDto>>;
