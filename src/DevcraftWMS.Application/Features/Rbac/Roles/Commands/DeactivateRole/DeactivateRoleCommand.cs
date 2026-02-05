using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Commands.DeactivateRole;

public sealed record DeactivateRoleCommand(Guid Id) : IRequest<RequestResult<string>>;
