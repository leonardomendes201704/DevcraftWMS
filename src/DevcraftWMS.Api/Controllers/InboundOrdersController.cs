using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.InboundOrders.Commands.ConvertAsnToInboundOrder;
using DevcraftWMS.Application.Features.InboundOrders.Commands.ApproveEmergencyInboundOrder;
using DevcraftWMS.Application.Features.InboundOrders.Commands.UpdateInboundOrderParameters;
using DevcraftWMS.Application.Features.InboundOrders.Commands.CancelInboundOrder;
using DevcraftWMS.Application.Features.InboundOrders.Commands.CompleteInboundOrder;
using DevcraftWMS.Application.Features.InboundOrders.Queries.ListInboundOrders;
using DevcraftWMS.Application.Features.InboundOrders.Queries.GetInboundOrder;
using DevcraftWMS.Application.Features.InboundOrders.Queries.GetInboundOrderReceiptReport;
using DevcraftWMS.Application.Features.InboundOrders.Queries.ExportInboundOrderReceiptReport;
using DevcraftWMS.Application.Features.InboundOrderNotifications.Queries.ListInboundOrderNotifications;
using DevcraftWMS.Application.Features.InboundOrderNotifications.Commands.ResendInboundOrderNotification;
using DevcraftWMS.Application.Features.ReceiptDivergences.Queries.ListInboundOrderDivergences;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api")]
public sealed class InboundOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public InboundOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("inbound-orders/from-asn")]
    public async Task<IActionResult> ConvertFromAsn([FromBody] ConvertAsnToInboundOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ConvertAsnToInboundOrderCommand(request.AsnId, request.Notes), cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("inbound-orders")]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] string? orderNumber = null,
        [FromQuery] InboundOrderStatus? status = null,
        [FromQuery] InboundOrderPriority? priority = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListInboundOrdersQuery(
                warehouseId,
                orderNumber,
                status,
                priority,
                isActive,
                includeInactive,
                pageNumber,
                pageSize,
                orderBy,
                orderDir),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("inbound-orders/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInboundOrderQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("inbound-orders/{id:guid}/divergences")]
    public async Task<IActionResult> ListDivergences(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListInboundOrderDivergencesQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("inbound-orders/{id:guid}/report")]
    public async Task<IActionResult> GetReceiptReport(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInboundOrderReceiptReportQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("inbound-orders/{id:guid}/report/export")]
    public async Task<IActionResult> ExportReceiptReport(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ExportInboundOrderReceiptReportQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return this.ToActionResult(result);
        }

        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HttpGet("inbound-orders/{id:guid}/notifications")]
    public async Task<IActionResult> ListNotifications(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListInboundOrderNotificationsQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("inbound-orders/{id:guid}/notifications/{notificationId:guid}/resend")]
    public async Task<IActionResult> ResendNotification(Guid id, Guid notificationId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ResendInboundOrderNotificationCommand(id, notificationId), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("inbound-orders/{id:guid}/parameters")]
    public async Task<IActionResult> UpdateParameters(Guid id, [FromBody] UpdateInboundOrderParametersRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateInboundOrderParametersCommand(
                id,
                (InboundOrderInspectionLevel)request.InspectionLevel,
                (InboundOrderPriority)request.Priority,
                request.SuggestedDock),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("inbound-orders/{id:guid}/approve-emergency")]
    public async Task<IActionResult> ApproveEmergency(Guid id, [FromBody] ApproveInboundOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ApproveEmergencyInboundOrderCommand(id, request.Notes), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("inbound-orders/{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelInboundOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelInboundOrderCommand(id, request.Reason), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("inbound-orders/{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteInboundOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteInboundOrderCommand(id, request.AllowPartial, request.Notes), cancellationToken);
        return this.ToActionResult(result);
    }
}
