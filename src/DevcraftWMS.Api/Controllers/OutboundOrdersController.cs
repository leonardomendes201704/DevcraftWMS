using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.OutboundOrders;
using DevcraftWMS.Application.Features.OutboundOrders.Commands.CreateOutboundOrder;
using DevcraftWMS.Application.Features.OutboundOrders.Commands.ReleaseOutboundOrder;
using DevcraftWMS.Application.Features.OutboundOrders.Queries.GetOutboundOrder;
using DevcraftWMS.Application.Features.OutboundOrders.Queries.ListOutboundOrders;
using DevcraftWMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api/outbound-orders")]
public sealed class OutboundOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OutboundOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOutboundOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateOutboundOrderCommand(
                request.WarehouseId,
                request.OrderNumber,
                request.CustomerReference,
                request.CarrierName,
                request.ExpectedShipDate,
                request.Notes,
                request.Items.Select(i => new CreateOutboundOrderItemInput(
                    i.ProductId,
                    i.UomId,
                    i.Quantity,
                    i.LotCode,
                    i.ExpirationDate)).ToList()),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] string? orderNumber = null,
        [FromQuery] OutboundOrderStatus? status = null,
        [FromQuery] OutboundOrderPriority? priority = null,
        [FromQuery] DateTime? createdFromUtc = null,
        [FromQuery] DateTime? createdToUtc = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListOutboundOrdersQuery(
                warehouseId,
                orderNumber,
                status,
                priority,
                createdFromUtc,
                createdToUtc,
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
        var result = await _mediator.Send(new GetOutboundOrderQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/release")]
    public async Task<IActionResult> Release(Guid id, [FromBody] ReleaseOutboundOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ReleaseOutboundOrderCommand(
                id,
                request.Priority,
                request.PickingMethod,
                request.ShippingWindowStartUtc,
                request.ShippingWindowEndUtc),
            cancellationToken);

        return this.ToActionResult(result);
    }
}
