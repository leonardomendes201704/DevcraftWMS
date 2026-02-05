using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Commands.UpdateRole;

public sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RequestResult<RoleDetailDto>>
{
    private readonly RoleService _service;

    public UpdateRoleCommandHandler(RoleService service)
    {
        _service = service;
    }

    public Task<RequestResult<RoleDetailDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        => _service.UpdateAsync(request.Id, request.Name, request.Description, request.PermissionIds, cancellationToken);
}
