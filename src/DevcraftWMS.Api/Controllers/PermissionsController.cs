using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Rbac.Permissions.Commands.CreatePermission;
using DevcraftWMS.Application.Features.Rbac.Permissions.Commands.UpdatePermission;
using DevcraftWMS.Application.Features.Rbac.Permissions.Commands.DeactivatePermission;
using DevcraftWMS.Application.Features.Rbac.Permissions.Queries.GetPermissionById;
using DevcraftWMS.Application.Features.Rbac.Permissions.Queries.ListPermissions;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Admin")]
[Route("api/permissions")]
public sealed class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ListPermissions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListPermissionsQuery(pageNumber, pageSize, orderBy, orderDir, search, isActive, includeInactive),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPermission(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPermissionByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreatePermissionCommand(request.Code, request.Name, request.Description), cancellationToken);
        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetPermission), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePermission(Guid id, [FromBody] UpdatePermissionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdatePermissionCommand(id, request.Code, request.Name, request.Description), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivatePermission(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivatePermissionCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
