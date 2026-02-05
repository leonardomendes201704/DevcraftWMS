using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Queries.GetPermissionById;

public sealed class GetPermissionByIdQueryHandler : IRequestHandler<GetPermissionByIdQuery, RequestResult<PermissionDto>>
{
    private readonly PermissionService _service;

    public GetPermissionByIdQueryHandler(PermissionService service)
    {
        _service = service;
    }

    public Task<RequestResult<PermissionDto>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.Id, cancellationToken);
}
