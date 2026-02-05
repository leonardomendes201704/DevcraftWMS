using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Commands.DeactivatePermission;

public sealed record DeactivatePermissionCommand(Guid Id) : IRequest<RequestResult<string>>;
