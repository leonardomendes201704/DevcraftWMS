using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.DockSchedules;
using DevcraftWMS.Application.Features.DockSchedules.Commands.AssignDockSchedule;
using DevcraftWMS.Application.Features.DockSchedules.Commands.CancelDockSchedule;
using DevcraftWMS.Application.Features.DockSchedules.Commands.CreateDockSchedule;
using DevcraftWMS.Application.Features.DockSchedules.Commands.RescheduleDockSchedule;
using DevcraftWMS.Application.Features.DockSchedules.Queries.GetDockScheduleById;
using DevcraftWMS.Application.Features.DockSchedules.Queries.ListDockSchedulesPaged;
using DevcraftWMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api/dock-schedules")]
[Authorize(Policy = "Role:Expedicao")]
public sealed class DockSchedulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DockSchedulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] string? dockCode = null,
        [FromQuery] DockScheduleStatus? status = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "SlotStartUtc",
        [FromQuery] string orderDir = "asc",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListDockSchedulesPagedQuery(
                warehouseId,
                dockCode,
                status,
                fromUtc,
                toUtc,
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
        var result = await _mediator.Send(new GetDockScheduleByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDockScheduleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateDockScheduleCommand(
                request.WarehouseId,
                request.DockCode,
                request.SlotStartUtc,
                request.SlotEndUtc,
                request.OutboundOrderId,
                request.Notes),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/reschedule")]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleDockScheduleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RescheduleDockScheduleCommand(id, request.SlotStartUtc, request.SlotEndUtc, request.Reason),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelDockScheduleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelDockScheduleCommand(id, request.Reason), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignDockScheduleRequest request, CancellationToken cancellationToken)
    {
        var assignment = new AssignDockScheduleInput(request.OutboundOrderId, request.OutboundShipmentId, request.Notes);
        var result = await _mediator.Send(new AssignDockScheduleCommand(id, assignment), cancellationToken);
        return this.ToActionResult(result);
    }
}
