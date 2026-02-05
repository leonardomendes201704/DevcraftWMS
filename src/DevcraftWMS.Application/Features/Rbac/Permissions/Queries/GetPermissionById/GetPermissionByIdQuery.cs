using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Queries.GetPermissionById;

public sealed record GetPermissionByIdQuery(Guid Id) : IRequest<RequestResult<PermissionDto>>;
