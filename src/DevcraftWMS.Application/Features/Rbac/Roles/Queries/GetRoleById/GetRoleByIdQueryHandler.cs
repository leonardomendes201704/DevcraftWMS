using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RequestResult<RoleDetailDto>>
{
    private readonly RoleService _service;

    public GetRoleByIdQueryHandler(RoleService service)
    {
        _service = service;
    }

    public Task<RequestResult<RoleDetailDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.Id, cancellationToken);
}
