using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Commands.CreatePermission;

public sealed class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, RequestResult<PermissionDto>>
{
    private readonly PermissionService _service;

    public CreatePermissionCommandHandler(PermissionService service)
    {
        _service = service;
    }

    public Task<RequestResult<PermissionDto>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        => _service.CreateAsync(request.Code, request.Name, request.Description, cancellationToken);
}
