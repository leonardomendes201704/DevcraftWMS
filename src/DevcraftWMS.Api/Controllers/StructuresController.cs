using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Structures.Commands.CreateStructure;
using DevcraftWMS.Application.Features.Structures.Commands.DeactivateStructure;
using DevcraftWMS.Application.Features.Structures.Commands.UpdateStructure;
using DevcraftWMS.Application.Features.Structures.Queries.GetStructureById;
using DevcraftWMS.Application.Features.Structures.Queries.ListStructuresForCustomerPaged;
using DevcraftWMS.Application.Features.Structures.Queries.ListStructuresPaged;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api")]
public sealed class StructuresController : ControllerBase
{
    private readonly IMediator _mediator;

    public StructuresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("sections/{sectionId:guid}/structures")]
    public async Task<IActionResult> CreateStructure(Guid sectionId, [FromBody] CreateStructureRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateStructureCommand(sectionId, request.Code, request.Name, request.StructureType, request.Levels),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetStructureById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("sections/{sectionId:guid}/structures")]
    public async Task<IActionResult> ListStructures(
        Guid sectionId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? code = null,
        [FromQuery] string? name = null,
        [FromQuery] StructureType? structureType = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListStructuresPagedQuery(null, null, sectionId, pageNumber, pageSize, orderBy, orderDir, code, name, structureType, isActive, includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("structures")]
    public async Task<IActionResult> ListStructuresForCustomer(
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] Guid? sectorId = null,
        [FromQuery] Guid? sectionId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? code = null,
        [FromQuery] string? name = null,
        [FromQuery] StructureType? structureType = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListStructuresForCustomerPagedQuery(warehouseId, sectorId, sectionId, pageNumber, pageSize, orderBy, orderDir, code, name, structureType, isActive, includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("structures/{id:guid}")]
    public async Task<IActionResult> GetStructureById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetStructureByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("structures/{id:guid}")]
    public async Task<IActionResult> UpdateStructure(Guid id, [FromBody] UpdateStructureRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateStructureCommand(id, request.SectionId, request.Code, request.Name, request.StructureType, request.Levels),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("structures/{id:guid}")]
    public async Task<IActionResult> DeactivateStructure(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateStructureCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
