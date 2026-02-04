using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.GateCheckins.Commands.CreateGateCheckin;
using DevcraftWMS.Application.Features.GateCheckins.Commands.UpdateGateCheckin;
using DevcraftWMS.Application.Features.GateCheckins.Commands.DeactivateGateCheckin;
using DevcraftWMS.Application.Features.GateCheckins.Commands.AssignDock;
using DevcraftWMS.Application.Features.GateCheckins.Queries.GetGateCheckin;
using DevcraftWMS.Application.Features.GateCheckins.Queries.ListGateCheckins;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Portaria")]
[Route("api/gate/checkins")]
public sealed class GateCheckinsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GateCheckinsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] Guid? inboundOrderId = null,
        [FromQuery] string? documentNumber = null,
        [FromQuery] string? vehiclePlate = null,
        [FromQuery] string? driverName = null,
        [FromQuery] string? carrierName = null,
        [FromQuery] GateCheckinStatus? status = null,
        [FromQuery] DateTime? arrivalFromUtc = null,
        [FromQuery] DateTime? arrivalToUtc = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListGateCheckinsQuery(
                inboundOrderId,
                documentNumber,
                vehiclePlate,
                driverName,
                carrierName,
                status,
                arrivalFromUtc,
                arrivalToUtc,
                isActive,
                includeInactive,
                pageNumber,
                pageSize,
                orderBy,
                orderDir),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetGateCheckinQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGateCheckinRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateGateCheckinCommand(
                request.InboundOrderId,
                request.DocumentNumber,
                request.VehiclePlate,
                request.DriverName,
                request.CarrierName,
                request.ArrivalAtUtc,
                request.Notes,
                request.WarehouseId),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGateCheckinRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateGateCheckinCommand(
                id,
                request.InboundOrderId,
                request.DocumentNumber,
                request.VehiclePlate,
                request.DriverName,
                request.CarrierName,
                request.ArrivalAtUtc,
                (GateCheckinStatus)request.Status,
                request.Notes),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/assign-dock")]
    public async Task<IActionResult> AssignDock(Guid id, [FromBody] AssignGateDockRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AssignGateDockCommand(id, request.DockCode), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateGateCheckinCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
