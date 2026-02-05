using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Commands.UpdatePermission;

public sealed class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, RequestResult<PermissionDto>>
{
    private readonly PermissionService _service;

    public UpdatePermissionCommandHandler(PermissionService service)
    {
        _service = service;
    }

    public Task<RequestResult<PermissionDto>> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        => _service.UpdateAsync(request.Id, request.Code, request.Name, request.Description, cancellationToken);
}
