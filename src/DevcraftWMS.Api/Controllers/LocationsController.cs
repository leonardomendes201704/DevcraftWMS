using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Locations.Commands.CreateLocation;
using DevcraftWMS.Application.Features.Locations.Commands.DeactivateLocation;
using DevcraftWMS.Application.Features.Locations.Commands.UpdateLocation;
using DevcraftWMS.Application.Features.Locations.Queries.GetLocationById;
using DevcraftWMS.Application.Features.Locations.Queries.ListLocationsPaged;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api")]
public sealed class LocationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("structures/{structureId:guid}/locations")]
    public async Task<IActionResult> CreateLocation(Guid structureId, [FromBody] CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateLocationCommand(
                structureId,
                request.ZoneId,
                request.Code,
                request.Barcode,
                request.Level,
                request.Row,
                request.Column,
                request.MaxWeightKg,
                request.MaxVolumeM3,
                request.AllowLotTracking,
                request.AllowExpiryTracking),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetLocationById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("structures/{structureId:guid}/locations")]
    public async Task<IActionResult> ListLocations(
        Guid structureId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] Guid? zoneId = null,
        [FromQuery] string? code = null,
        [FromQuery] string? barcode = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListLocationsPagedQuery(structureId, zoneId, pageNumber, pageSize, orderBy, orderDir, code, barcode, isActive, includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("locations/{id:guid}")]
    public async Task<IActionResult> GetLocationById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLocationByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("locations/{id:guid}")]
    public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateLocationCommand(
                id,
                request.StructureId,
                request.ZoneId,
                request.Code,
                request.Barcode,
                request.Level,
                request.Row,
                request.Column,
                request.MaxWeightKg,
                request.MaxVolumeM3,
                request.AllowLotTracking,
                request.AllowExpiryTracking),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("locations/{id:guid}")]
    public async Task<IActionResult> DeactivateLocation(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateLocationCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
