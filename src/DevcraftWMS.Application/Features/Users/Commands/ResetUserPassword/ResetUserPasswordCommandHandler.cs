using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Users.Commands.ResetUserPassword;

public sealed class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, RequestResult<string>>
{
    private readonly UserManagementService _service;

    public ResetUserPasswordCommandHandler(UserManagementService service)
    {
        _service = service;
    }

    public Task<RequestResult<string>> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
        => _service.ResetPasswordAsync(request.Id, cancellationToken);
}
