using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Sectors.Commands.CreateSector;
using DevcraftWMS.Application.Features.Sectors.Commands.DeactivateSector;
using DevcraftWMS.Application.Features.Sectors.Commands.UpdateSector;
using DevcraftWMS.Application.Features.Sectors.Queries.GetSectorById;
using DevcraftWMS.Application.Features.Sectors.Queries.ListSectorsPaged;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api")]
public sealed class SectorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SectorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("warehouses/{warehouseId:guid}/sectors")]
    public async Task<IActionResult> CreateSector(Guid warehouseId, [FromBody] CreateSectorRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateSectorCommand(warehouseId, request.Code, request.Name, request.Description, request.SectorType),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetSectorById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("warehouses/{warehouseId:guid}/sectors")]
    public async Task<IActionResult> ListSectors(
        Guid warehouseId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? code = null,
        [FromQuery] string? name = null,
        [FromQuery] SectorType? sectorType = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListSectorsPagedQuery(warehouseId, pageNumber, pageSize, orderBy, orderDir, code, name, sectorType, isActive, includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("sectors/{id:guid}")]
    public async Task<IActionResult> GetSectorById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSectorByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("sectors/{id:guid}")]
    public async Task<IActionResult> UpdateSector(Guid id, [FromBody] UpdateSectorRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateSectorCommand(id, request.WarehouseId, request.Code, request.Name, request.Description, request.SectorType),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("sectors/{id:guid}")]
    public async Task<IActionResult> DeactivateSector(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateSectorCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
