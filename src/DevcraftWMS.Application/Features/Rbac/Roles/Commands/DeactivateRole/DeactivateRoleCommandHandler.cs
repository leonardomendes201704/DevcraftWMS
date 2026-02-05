using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Commands.DeactivateRole;

public sealed class DeactivateRoleCommandHandler : IRequestHandler<DeactivateRoleCommand, RequestResult<string>>
{
    private readonly RoleService _service;

    public DeactivateRoleCommandHandler(RoleService service)
    {
        _service = service;
    }

    public Task<RequestResult<string>> Handle(DeactivateRoleCommand request, CancellationToken cancellationToken)
        => _service.DeactivateAsync(request.Id, cancellationToken);
}
