using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.OutboundChecks.Commands.StartOutboundCheck;
using DevcraftWMS.Application.Features.OutboundChecks.Queries.ListOutboundChecksPaged;
using DevcraftWMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api/outbound-checks")]
public sealed class OutboundChecksController : ControllerBase
{
    private readonly IMediator _mediator;

    public OutboundChecksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] Guid? outboundOrderId = null,
        [FromQuery] OutboundCheckStatus? status = null,
        [FromQuery] OutboundOrderPriority? priority = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListOutboundChecksPagedQuery(
                warehouseId,
                outboundOrderId,
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

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new StartOutboundCheckCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
