using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Users.Commands.AssignUserRoles;

public sealed class AssignUserRolesCommandHandler : IRequestHandler<AssignUserRolesCommand, RequestResult<string>>
{
    private readonly UserManagementService _service;

    public AssignUserRolesCommandHandler(UserManagementService service)
    {
        _service = service;
    }

    public Task<RequestResult<string>> Handle(AssignUserRolesCommand request, CancellationToken cancellationToken)
        => _service.AssignRolesAsync(request.Id, request.RoleIds, cancellationToken);
}
