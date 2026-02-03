using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Receipts.Commands.CreateReceipt;
using DevcraftWMS.Application.Features.Receipts.Commands.UpdateReceipt;
using DevcraftWMS.Application.Features.Receipts.Commands.DeactivateReceipt;
using DevcraftWMS.Application.Features.Receipts.Commands.CompleteReceipt;
using DevcraftWMS.Application.Features.Receipts.Commands.StartReceiptFromInboundOrder;
using DevcraftWMS.Application.Features.Receipts.Commands.AddReceiptItem;
using DevcraftWMS.Application.Features.Receipts.Queries.GetReceiptById;
using DevcraftWMS.Application.Features.Receipts.Queries.ListReceiptsPaged;
using DevcraftWMS.Application.Features.Receipts.Queries.ListReceiptItemsPaged;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api")]
public sealed class ReceiptsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReceiptsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("receipts")]
    public async Task<IActionResult> CreateReceipt([FromBody] CreateReceiptRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateReceiptCommand(
                request.WarehouseId,
                request.ReceiptNumber,
                request.DocumentNumber,
                request.SupplierName,
                request.Notes),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetReceiptById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpPost("inbound-orders/{id:guid}/receipts/start")]
    public async Task<IActionResult> StartFromInboundOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new StartReceiptFromInboundOrderCommand(id), cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetReceiptById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("receipts")]
    public async Task<IActionResult> ListReceipts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] string? receiptNumber = null,
        [FromQuery] string? documentNumber = null,
        [FromQuery] string? supplierName = null,
        [FromQuery] ReceiptStatus? status = null,
        [FromQuery] DateOnly? receivedFrom = null,
        [FromQuery] DateOnly? receivedTo = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListReceiptsPagedQuery(
                pageNumber,
                pageSize,
                orderBy,
                orderDir,
                warehouseId,
                receiptNumber,
                documentNumber,
                supplierName,
                status,
                receivedFrom,
                receivedTo,
                isActive,
                includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("receipts/{id:guid}")]
    public async Task<IActionResult> GetReceiptById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetReceiptByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("receipts/{id:guid}")]
    public async Task<IActionResult> UpdateReceipt(Guid id, [FromBody] UpdateReceiptRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateReceiptCommand(
                id,
                request.WarehouseId,
                request.ReceiptNumber,
                request.DocumentNumber,
                request.SupplierName,
                request.Notes),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("receipts/{id:guid}")]
    public async Task<IActionResult> DeactivateReceipt(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateReceiptCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("receipts/{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, [FromBody] AddReceiptItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddReceiptItemCommand(
                id,
                request.ProductId,
                request.LotId,
                request.LocationId,
                request.UomId,
                request.Quantity,
                request.UnitCost),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("receipts/{id:guid}/items")]
    public async Task<IActionResult> ListItems(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] Guid? productId = null,
        [FromQuery] Guid? locationId = null,
        [FromQuery] Guid? lotId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListReceiptItemsPagedQuery(
                id,
                pageNumber,
                pageSize,
                orderBy,
                orderDir,
                productId,
                locationId,
                lotId,
                isActive,
                includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("receipts/{id:guid}/complete")]
    public async Task<IActionResult> CompleteReceipt(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteReceiptCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("receipts/{id:guid}/finish")]
    public async Task<IActionResult> FinishReceipt(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteReceiptCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
