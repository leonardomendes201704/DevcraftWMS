using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.PickingReplenishments.Commands.GeneratePickingReplenishments;
using DevcraftWMS.Application.Features.PickingReplenishments.Queries.ListPickingReplenishmentsPaged;
using DevcraftWMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api/picking-replenishments")]
public sealed class PickingReplenishmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PickingReplenishmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] Guid? productId = null,
        [FromQuery] PickingReplenishmentStatus? status = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListPickingReplenishmentsPagedQuery(
                warehouseId,
                productId,
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

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        [FromBody] GeneratePickingReplenishmentsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GeneratePickingReplenishmentsCommand(request.WarehouseId),
            cancellationToken);

        return this.ToActionResult(result);
    }
}
