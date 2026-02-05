using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RequestResult<RoleDetailDto>>
{
    private readonly RoleService _service;

    public CreateRoleCommandHandler(RoleService service)
    {
        _service = service;
    }

    public Task<RequestResult<RoleDetailDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        => _service.CreateAsync(request.Name, request.Description, request.PermissionIds, cancellationToken);
}
