using MediatR;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.InventoryMovements.Commands.CreateInventoryMovement;
using DevcraftWMS.Application.Features.InventoryMovements.Queries.GetInventoryMovementById;
using DevcraftWMS.Application.Features.InventoryMovements.Queries.ListInventoryMovementsPaged;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api/inventory/movements")]
public sealed class InventoryMovementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryMovementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "PerformedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] Guid? productId = null,
        [FromQuery] Guid? fromLocationId = null,
        [FromQuery] Guid? toLocationId = null,
        [FromQuery] Guid? lotId = null,
        [FromQuery] InventoryMovementStatus? status = null,
        [FromQuery] DateTime? performedFromUtc = null,
        [FromQuery] DateTime? performedToUtc = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListInventoryMovementsPagedQuery(
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            productId,
            fromLocationId,
            toLocationId,
            lotId,
            status,
            performedFromUtc,
            performedToUtc,
            isActive,
            includeInactive), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInventoryMovementByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInventoryMovementRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateInventoryMovementCommand(
            request.FromLocationId,
            request.ToLocationId,
            request.ProductId,
            request.LotId,
            request.Quantity,
            request.Reason,
            request.Reference,
            request.PerformedAtUtc), cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }
}
