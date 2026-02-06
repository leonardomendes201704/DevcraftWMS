using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.InventoryVisibility.Queries.ExportInventoryVisibility;
using DevcraftWMS.Application.Features.InventoryVisibility.Queries.GetInventoryVisibility;
using DevcraftWMS.Application.Features.InventoryVisibility.Queries.GetInventoryVisibilityTimeline;
using DevcraftWMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api/inventory-visibility")]
[Authorize(Policy = "Role:Backoffice")]
public sealed class InventoryVisibilityController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryVisibilityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] Guid customerId,
        [FromQuery] Guid warehouseId,
        [FromQuery] Guid? productId = null,
        [FromQuery] string? sku = null,
        [FromQuery] string? lotCode = null,
        [FromQuery] DateOnly? expirationFrom = null,
        [FromQuery] DateOnly? expirationTo = null,
        [FromQuery] InventoryBalanceStatus? status = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string orderBy = "ProductCode",
        [FromQuery] string orderDir = "asc",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetInventoryVisibilityQuery(
                customerId,
                warehouseId,
                productId,
                sku,
                lotCode,
                expirationFrom,
                expirationTo,
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

    [HttpGet("{productId:guid}/timeline")]
    public async Task<IActionResult> Timeline(
        Guid productId,
        [FromQuery] Guid customerId,
        [FromQuery] Guid warehouseId,
        [FromQuery] string? lotCode = null,
        [FromQuery] Guid? locationId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetInventoryVisibilityTimelineQuery(
                customerId,
                warehouseId,
                productId,
                lotCode,
                locationId),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] Guid customerId,
        [FromQuery] Guid warehouseId,
        [FromQuery] Guid? productId = null,
        [FromQuery] string? sku = null,
        [FromQuery] string? lotCode = null,
        [FromQuery] DateOnly? expirationFrom = null,
        [FromQuery] DateOnly? expirationTo = null,
        [FromQuery] Domain.Enums.InventoryBalanceStatus? status = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string format = "print",
        [FromQuery] string orderBy = "ProductCode",
        [FromQuery] string orderDir = "asc",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ExportInventoryVisibilityQuery(
                customerId,
                warehouseId,
                productId,
                sku,
                lotCode,
                expirationFrom,
                expirationTo,
                status,
                isActive,
                includeInactive,
                format,
                orderBy,
                orderDir),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return this.ToActionResult(result);
        }

        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }
}
