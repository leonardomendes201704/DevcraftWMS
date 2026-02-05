using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Users.Commands.DeactivateUser;

public sealed class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, RequestResult<string>>
{
    private readonly UserManagementService _service;

    public DeactivateUserCommandHandler(UserManagementService service)
    {
        _service = service;
    }

    public Task<RequestResult<string>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        => _service.DeactivateAsync(request.Id, cancellationToken);
}
