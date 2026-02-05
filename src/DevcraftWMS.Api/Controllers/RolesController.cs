using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Rbac.Roles.Commands.CreateRole;
using DevcraftWMS.Application.Features.Rbac.Roles.Commands.UpdateRole;
using DevcraftWMS.Application.Features.Rbac.Roles.Commands.DeactivateRole;
using DevcraftWMS.Application.Features.Rbac.Roles.Queries.GetRoleById;
using DevcraftWMS.Application.Features.Rbac.Roles.Queries.ListRoles;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Admin")]
[Route("api/roles")]
public sealed class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ListRoles(
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
            new ListRolesQuery(search, isActive, includeInactive, pageNumber, pageSize, orderBy, orderDir),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRole(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRoleByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateRoleCommand(request.Name, request.Description, request.PermissionIds), cancellationToken);
        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetRole), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateRoleCommand(id, request.Name, request.Description, request.PermissionIds), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateRole(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateRoleCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
