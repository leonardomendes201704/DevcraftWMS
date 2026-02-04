using DevcraftWMS.Application.Features.PutawayTasks.Queries.GetPutawayTaskById;
using DevcraftWMS.Application.Features.PutawayTasks.Queries.GetPutawayTaskSuggestions;
using DevcraftWMS.Application.Features.PutawayTasks.Queries.ListPutawayTasksPaged;
using DevcraftWMS.Application.Features.PutawayTasks.Commands.ConfirmPutawayTask;
using DevcraftWMS.Application.Features.PutawayTasks.Commands.ReassignPutawayTask;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api/putaway-tasks")]
public sealed class PutawayTasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public PutawayTasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "Role:Backoffice")]
    public async Task<IActionResult> List(
        [FromQuery] Guid? warehouseId,
        [FromQuery] Guid? receiptId,
        [FromQuery] Guid? unitLoadId,
        [FromQuery] PutawayTaskStatus? status,
        [FromQuery] bool? isActive,
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListPutawayTasksPagedQuery(
            warehouseId,
            receiptId,
            unitLoadId,
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
    [Authorize(Policy = "Role:Backoffice")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPutawayTaskByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}/suggestions")]
    [Authorize(Policy = "Role:Backoffice")]
    public async Task<IActionResult> GetSuggestions(Guid id, [FromQuery] int limit = 5, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetPutawayTaskSuggestionsQuery(id, limit), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/confirm")]
    [Authorize(Policy = "Role:Backoffice")]
    public async Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmPutawayTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ConfirmPutawayTaskCommand(id, request.LocationId, request.Notes),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/reassign")]
    [Authorize(Policy = "Role:Backoffice")]
    public async Task<IActionResult> Reassign(Guid id, [FromBody] ReassignPutawayTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ReassignPutawayTaskCommand(id, request.AssigneeEmail, request.Reason),
            cancellationToken);

        return this.ToActionResult(result);
    }
}
