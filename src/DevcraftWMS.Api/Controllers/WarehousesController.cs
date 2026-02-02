using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Warehouses;
using DevcraftWMS.Application.Features.Warehouses.Commands.CreateWarehouse;
using DevcraftWMS.Application.Features.Warehouses.Commands.UpdateWarehouse;
using DevcraftWMS.Application.Features.Warehouses.Commands.DeactivateWarehouse;
using DevcraftWMS.Application.Features.Warehouses.Queries.GetWarehouseById;
using DevcraftWMS.Application.Features.Warehouses.Queries.ListWarehousesPaged;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api/warehouses")]
public sealed class WarehousesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehousesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? search = null,
        [FromQuery] string? code = null,
        [FromQuery] string? name = null,
        [FromQuery] DevcraftWMS.Domain.Enums.WarehouseType? warehouseType = null,
        [FromQuery] string? city = null,
        [FromQuery] string? state = null,
        [FromQuery] string? country = null,
        [FromQuery] string? externalId = null,
        [FromQuery] string? erpCode = null,
        [FromQuery] string? costCenterCode = null,
        [FromQuery] bool? isPrimary = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListWarehousesPagedQuery(
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            search,
            code,
            name,
            warehouseType,
            city,
            state,
            country,
            externalId,
            erpCode,
            costCenterCode,
            isPrimary,
            includeInactive), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWarehouseByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateWarehouseCommand(
            request.Code,
            request.Name,
            request.ShortName,
            request.Description,
            request.WarehouseType,
            request.IsPrimary,
            request.IsPickingEnabled,
            request.IsReceivingEnabled,
            request.IsShippingEnabled,
            request.IsReturnsEnabled,
            request.ExternalId,
            request.ErpCode,
            request.CostCenterCode,
            request.CostCenterName,
            request.CutoffTime,
            request.Timezone,
            MapAddress(request.Address),
            MapContact(request.Contact),
            MapCapacity(request.Capacity)), cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateWarehouseCommand(
            id,
            request.Code,
            request.Name,
            request.ShortName,
            request.Description,
            request.WarehouseType,
            request.IsPrimary,
            request.IsPickingEnabled,
            request.IsReceivingEnabled,
            request.IsShippingEnabled,
            request.IsReturnsEnabled,
            request.ExternalId,
            request.ErpCode,
            request.CostCenterCode,
            request.CostCenterName,
            request.CutoffTime,
            request.Timezone,
            MapAddress(request.Address),
            MapContact(request.Contact),
            MapCapacity(request.Capacity)), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateWarehouseCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }

    private static WarehouseAddressInput? MapAddress(WarehouseAddressRequest? request)
    {
        return request is null
            ? null
            : new WarehouseAddressInput(
                request.AddressLine1,
                request.AddressLine2,
                request.District,
                request.City,
                request.State,
                request.PostalCode,
                request.Country,
                request.Latitude,
                request.Longitude);
    }

    private static WarehouseContactInput? MapContact(WarehouseContactRequest? request)
    {
        return request is null
            ? null
            : new WarehouseContactInput(
                request.ContactName,
                request.ContactEmail,
                request.ContactPhone);
    }

    private static WarehouseCapacityInput? MapCapacity(WarehouseCapacityRequest? request)
    {
        return request is null
            ? null
            : new WarehouseCapacityInput(
                request.LengthMeters,
                request.WidthMeters,
                request.HeightMeters,
                request.TotalAreaM2,
                request.TotalCapacity,
                request.CapacityUnit,
                request.MaxWeightKg,
                request.OperationalArea);
    }
}
