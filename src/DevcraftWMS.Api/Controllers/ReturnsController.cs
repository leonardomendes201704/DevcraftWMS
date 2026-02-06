using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Returns;
using DevcraftWMS.Application.Features.Returns.Commands.CompleteReturnOrder;
using DevcraftWMS.Application.Features.Returns.Commands.CreateReturnOrder;
using DevcraftWMS.Application.Features.Returns.Commands.ReceiveReturnOrder;
using DevcraftWMS.Application.Features.Returns.Queries.GetReturnById;
using DevcraftWMS.Application.Features.Returns.Queries.ListReturnsPaged;
using DevcraftWMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api/returns")]
[Authorize(Policy = "Role:Backoffice")]
public sealed class ReturnsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReturnsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] string? returnNumber = null,
        [FromQuery] ReturnStatus? status = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListReturnsPagedQuery(
                warehouseId,
                returnNumber,
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
        var result = await _mediator.Send(new GetReturnByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReturnOrderRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new CreateReturnItemInput(
            i.ProductId,
            i.UomId,
            i.LotCode,
            i.ExpirationDate,
            i.QuantityExpected)).ToList();

        var result = await _mediator.Send(
            new CreateReturnOrderCommand(
                request.WarehouseId,
                request.ReturnNumber,
                request.OutboundOrderId,
                request.Notes,
                items),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/receive")]
    public async Task<IActionResult> Receive(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ReceiveReturnOrderCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteReturnOrderRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new CompleteReturnItemInput(
            i.ReturnItemId,
            i.QuantityReceived,
            i.Disposition,
            i.DispositionNotes,
            i.LocationId)).ToList();

        var result = await _mediator.Send(
            new CompleteReturnOrderCommand(
                id,
                items,
                request.Notes),
            cancellationToken);

        return this.ToActionResult(result);
    }
}
