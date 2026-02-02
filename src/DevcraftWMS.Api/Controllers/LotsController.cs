using MediatR;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Lots.Commands.CreateLot;
using DevcraftWMS.Application.Features.Lots.Commands.UpdateLot;
using DevcraftWMS.Application.Features.Lots.Commands.DeactivateLot;
using DevcraftWMS.Application.Features.Lots.Queries.GetLotById;
using DevcraftWMS.Application.Features.Lots.Queries.ListLotsPaged;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class LotsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LotsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("products/{productId:guid}/lots")]
    public async Task<IActionResult> CreateLot(Guid productId, [FromBody] CreateLotRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateLotCommand(
                productId,
                request.Code,
                request.ManufactureDate,
                request.ExpirationDate,
                request.Status),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetLotById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("products/{productId:guid}/lots")]
    public async Task<IActionResult> ListLots(
        Guid productId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? code = null,
        [FromQuery] LotStatus? status = null,
        [FromQuery] DateOnly? expirationFrom = null,
        [FromQuery] DateOnly? expirationTo = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListLotsPagedQuery(
                productId,
                pageNumber,
                pageSize,
                orderBy,
                orderDir,
                code,
                status,
                expirationFrom,
                expirationTo,
                isActive,
                includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("lots/{id:guid}")]
    public async Task<IActionResult> GetLotById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLotByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("lots/{id:guid}")]
    public async Task<IActionResult> UpdateLot(Guid id, [FromBody] UpdateLotRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateLotCommand(
                id,
                request.ProductId,
                request.Code,
                request.ManufactureDate,
                request.ExpirationDate,
                request.Status),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("lots/{id:guid}")]
    public async Task<IActionResult> DeactivateLot(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateLotCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
