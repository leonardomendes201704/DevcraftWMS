using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Zones.Commands.CreateZone;
using DevcraftWMS.Application.Features.Zones.Commands.DeactivateZone;
using DevcraftWMS.Application.Features.Zones.Commands.UpdateZone;
using DevcraftWMS.Application.Features.Zones.Queries.GetZoneById;
using DevcraftWMS.Application.Features.Zones.Queries.ListZonesPaged;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api")]
public sealed class ZonesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ZonesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("warehouses/{warehouseId:guid}/zones")]
    public async Task<IActionResult> CreateZone(Guid warehouseId, [FromBody] CreateZoneRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateZoneCommand(warehouseId, request.Code, request.Name, request.Description, request.ZoneType),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetZoneById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("warehouses/{warehouseId:guid}/zones")]
    public async Task<IActionResult> ListZones(
        Guid warehouseId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? code = null,
        [FromQuery] string? name = null,
        [FromQuery] ZoneType? zoneType = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListZonesPagedQuery(warehouseId, pageNumber, pageSize, orderBy, orderDir, code, name, zoneType, isActive, includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("zones/{id:guid}")]
    public async Task<IActionResult> GetZoneById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetZoneByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("zones/{id:guid}")]
    public async Task<IActionResult> UpdateZone(Guid id, [FromBody] UpdateZoneRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateZoneCommand(id, request.WarehouseId, request.Code, request.Name, request.Description, request.ZoneType),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("zones/{id:guid}")]
    public async Task<IActionResult> DeactivateZone(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateZoneCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
