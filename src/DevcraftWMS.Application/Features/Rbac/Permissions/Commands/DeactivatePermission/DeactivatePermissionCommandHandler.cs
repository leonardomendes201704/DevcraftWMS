using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Commands.DeactivatePermission;

public sealed class DeactivatePermissionCommandHandler : IRequestHandler<DeactivatePermissionCommand, RequestResult<string>>
{
    private readonly PermissionService _service;

    public DeactivatePermissionCommandHandler(PermissionService service)
    {
        _service = service;
    }

    public Task<RequestResult<string>> Handle(DeactivatePermissionCommand request, CancellationToken cancellationToken)
        => _service.DeactivateAsync(request.Id, cancellationToken);
}
