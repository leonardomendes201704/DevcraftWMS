using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, RequestResult<UserDetailDto>>
{
    private readonly UserManagementService _service;

    public UpdateUserCommandHandler(UserManagementService service)
    {
        _service = service;
    }

    public Task<RequestResult<UserDetailDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        => _service.UpdateAsync(request.Id, request.Email, request.FullName, request.RoleIds, cancellationToken);
}
