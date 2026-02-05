using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.OutboundOrders;
using DevcraftWMS.Application.Features.OutboundOrders.Commands.CreateOutboundOrder;
using DevcraftWMS.Application.Features.OutboundOrders.Commands.ReleaseOutboundOrder;
using DevcraftWMS.Application.Features.OutboundChecks;
using DevcraftWMS.Application.Features.OutboundChecks.Commands.RegisterOutboundCheck;
using DevcraftWMS.Application.Features.OutboundPacking;
using DevcraftWMS.Application.Features.OutboundPacking.Commands.RegisterOutboundPacking;
using DevcraftWMS.Application.Features.OutboundPacking.Queries.ListOutboundPackages;
using DevcraftWMS.Application.Features.OutboundShipping;
using DevcraftWMS.Application.Features.OutboundShipping.Commands.RegisterOutboundShipment;
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

    [HttpPost("{id:guid}/check")]
    public async Task<IActionResult> Check(Guid id, [FromBody] RegisterOutboundCheckRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new OutboundCheckItemInput(
            i.OutboundOrderItemId,
            i.QuantityChecked,
            i.DivergenceReason,
            i.Evidence?.Select(e => new OutboundCheckEvidenceInput(e.FileName, e.ContentType, e.SizeBytes, e.Content)).ToList()));

        var result = await _mediator.Send(
            new RegisterOutboundCheckCommand(
                id,
                items.ToList(),
                request.Notes),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/pack")]
    public async Task<IActionResult> Pack(Guid id, [FromBody] RegisterOutboundPackingRequest request, CancellationToken cancellationToken)
    {
        var packages = request.Packages.Select(p => new OutboundPackageInput(
            p.PackageNumber,
            p.WeightKg,
            p.LengthCm,
            p.WidthCm,
            p.HeightCm,
            p.Notes,
            p.Items.Select(i => new OutboundPackageItemInput(i.OutboundOrderItemId, i.Quantity)).ToList()))
            .ToList();

        var result = await _mediator.Send(
            new RegisterOutboundPackingCommand(id, packages),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}/packages")]
    public async Task<IActionResult> ListPackages(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListOutboundPackagesQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/ship")]
    public async Task<IActionResult> Ship(Guid id, [FromBody] RegisterOutboundShipmentRequest request, CancellationToken cancellationToken)
    {
        var input = new RegisterOutboundShipmentInput(
            request.DockCode,
            request.LoadingStartedAtUtc,
            request.LoadingCompletedAtUtc,
            request.ShippedAtUtc,
            request.Notes,
            request.Packages.Select(p => new OutboundShipmentPackageInput(p.OutboundPackageId)).ToList());

        var result = await _mediator.Send(
            new RegisterOutboundShipmentCommand(id, input),
            cancellationToken);

        return this.ToActionResult(result);
    }
}
