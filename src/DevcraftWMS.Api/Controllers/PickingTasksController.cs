using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.PickingTasks.Commands.ConfirmPickingTask;
using DevcraftWMS.Application.Features.PickingTasks.Commands.StartPickingTask;
using DevcraftWMS.Application.Features.PickingTasks.Queries.GetPickingTaskById;
using DevcraftWMS.Application.Features.PickingTasks.Queries.ListPickingTasksPaged;
using DevcraftWMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api/picking-tasks")]
public sealed class PickingTasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public PickingTasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "Role:Picking")]
    public async Task<IActionResult> List(
        [FromQuery] Guid? warehouseId,
        [FromQuery] Guid? outboundOrderId,
        [FromQuery] Guid? assignedUserId,
        [FromQuery] PickingTaskStatus? status,
        [FromQuery] bool? isActive,
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListPickingTasksPagedQuery(
            warehouseId,
            outboundOrderId,
            assignedUserId,
            status,
            isActive,
            includeInactive,
            pageNumber,
            pageSize,
            orderBy,
            orderDir), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Role:Picking")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPickingTaskByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/start")]
    [Authorize(Policy = "Role:Picking")]
    public async Task<IActionResult> Start(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new StartPickingTaskCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/confirm")]
    [Authorize(Policy = "Role:Picking")]
    public async Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmPickingTaskRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new ConfirmPickingTaskItemInput(i.PickingTaskItemId, i.QuantityPicked)).ToList();
        var result = await _mediator.Send(new ConfirmPickingTaskCommand(id, items, request.Notes), cancellationToken);
        return this.ToActionResult(result);
    }
}
