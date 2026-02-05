using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, RequestResult<UserDetailDto>>
{
    private readonly UserManagementService _service;

    public GetUserByIdQueryHandler(UserManagementService service)
    {
        _service = service;
    }

    public Task<RequestResult<UserDetailDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.Id, cancellationToken);
}
