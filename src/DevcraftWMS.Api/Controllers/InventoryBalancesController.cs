using MediatR;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.InventoryBalances.Commands.CreateInventoryBalance;
using DevcraftWMS.Application.Features.InventoryBalances.Commands.UpdateInventoryBalance;
using DevcraftWMS.Application.Features.InventoryBalances.Commands.DeactivateInventoryBalance;
using DevcraftWMS.Application.Features.InventoryBalances.Queries.GetInventoryBalanceById;
using DevcraftWMS.Application.Features.InventoryBalances.Queries.ListInventoryBalances;
using DevcraftWMS.Application.Features.InventoryBalances.Queries.ListLocationInventory;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class InventoryBalancesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryBalancesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("locations/{locationId:guid}/inventory")]
    public async Task<IActionResult> CreateForLocation(Guid locationId, [FromBody] CreateInventoryBalanceRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateInventoryBalanceCommand(
                locationId,
                request.ProductId,
                request.LotId,
                request.QuantityOnHand,
                request.QuantityReserved,
                request.Status),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("locations/{locationId:guid}/inventory")]
    public async Task<IActionResult> ListByLocation(
        Guid locationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] Guid? productId = null,
        [FromQuery] Guid? lotId = null,
        [FromQuery] InventoryBalanceStatus? status = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListLocationInventoryQuery(
                locationId,
                pageNumber,
                pageSize,
                orderBy,
                orderDir,
                productId,
                lotId,
                status,
                isActive,
                includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("inventory/balances")]
    public async Task<IActionResult> ListBalances(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] Guid? locationId = null,
        [FromQuery] Guid? productId = null,
        [FromQuery] Guid? lotId = null,
        [FromQuery] InventoryBalanceStatus? status = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListInventoryBalancesQuery(
                pageNumber,
                pageSize,
                orderBy,
                orderDir,
                locationId,
                productId,
                lotId,
                status,
                isActive,
                includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("inventory/balances/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInventoryBalanceByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("inventory/balances/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInventoryBalanceRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateInventoryBalanceCommand(
                id,
                request.LocationId,
                request.ProductId,
                request.LotId,
                request.QuantityOnHand,
                request.QuantityReserved,
                request.Status),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("inventory/balances/{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateInventoryBalanceCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
