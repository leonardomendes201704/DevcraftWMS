using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.InventoryCounts;
using DevcraftWMS.Application.Features.InventoryCounts.Commands.CompleteInventoryCount;
using DevcraftWMS.Application.Features.InventoryCounts.Commands.CreateInventoryCount;
using DevcraftWMS.Application.Features.InventoryCounts.Commands.StartInventoryCount;
using DevcraftWMS.Application.Features.InventoryCounts.Queries.GetInventoryCountById;
using DevcraftWMS.Application.Features.InventoryCounts.Queries.ListInventoryCountsPaged;
using DevcraftWMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api/inventory-counts")]
[Authorize(Policy = "Role:Backoffice")]
public sealed class InventoryCountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryCountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] Guid? locationId = null,
        [FromQuery] InventoryCountStatus? status = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListInventoryCountsPagedQuery(
                warehouseId,
                locationId,
                status,
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
        var result = await _mediator.Send(new GetInventoryCountByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInventoryCountRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateInventoryCountCommand(
                request.WarehouseId,
                request.LocationId,
                request.ZoneId,
                request.Notes),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new StartInventoryCountCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteInventoryCountRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new CompleteInventoryCountItemInput(i.InventoryCountItemId, i.QuantityCounted)).ToList();
        var result = await _mediator.Send(
            new CompleteInventoryCountCommand(id, items, request.Notes),
            cancellationToken);

        return this.ToActionResult(result);
    }
}
