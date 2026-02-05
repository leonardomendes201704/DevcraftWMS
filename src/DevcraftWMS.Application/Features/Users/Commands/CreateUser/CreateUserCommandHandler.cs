using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, RequestResult<UserDetailDto>>
{
    private readonly UserManagementService _service;

    public CreateUserCommandHandler(UserManagementService service)
    {
        _service = service;
    }

    public Task<RequestResult<UserDetailDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        => _service.CreateAsync(request.Email, request.FullName, request.Password, request.RoleIds, cancellationToken);
}
