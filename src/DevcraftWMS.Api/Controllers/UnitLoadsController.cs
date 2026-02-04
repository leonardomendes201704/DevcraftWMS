using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.UnitLoads.Commands.CreateUnitLoad;
using DevcraftWMS.Application.Features.UnitLoads.Commands.PrintUnitLoadLabel;
using DevcraftWMS.Application.Features.UnitLoads.Commands.RelabelUnitLoadLabel;
using DevcraftWMS.Application.Features.UnitLoads.Queries.GetUnitLoadById;
using DevcraftWMS.Application.Features.UnitLoads.Queries.ListUnitLoadsPaged;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api")]
public sealed class UnitLoadsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UnitLoadsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("unit-loads")]
    public async Task<IActionResult> Create([FromBody] CreateUnitLoadRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateUnitLoadCommand(request.ReceiptId, request.SsccExternal, request.Notes),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("unit-loads")]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] Guid? receiptId = null,
        [FromQuery] string? sscc = null,
        [FromQuery] UnitLoadStatus? status = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListUnitLoadsPagedQuery(
                pageNumber,
                pageSize,
                orderBy,
                orderDir,
                warehouseId,
                receiptId,
                sscc,
                status,
                isActive,
                includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("unit-loads/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUnitLoadByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("unit-loads/{id:guid}/print")]
    public async Task<IActionResult> PrintLabel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new PrintUnitLoadLabelCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("unit-loads/{id:guid}/relabel")]
    public async Task<IActionResult> Relabel(Guid id, [FromBody] RelabelUnitLoadRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RelabelUnitLoadLabelCommand(id, request.Reason, request.Notes), cancellationToken);
        return this.ToActionResult(result);
    }
}
