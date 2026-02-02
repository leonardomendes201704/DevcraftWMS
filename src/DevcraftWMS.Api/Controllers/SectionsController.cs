using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Sections.Commands.CreateSection;
using DevcraftWMS.Application.Features.Sections.Commands.DeactivateSection;
using DevcraftWMS.Application.Features.Sections.Commands.UpdateSection;
using DevcraftWMS.Application.Features.Sections.Queries.GetSectionById;
using DevcraftWMS.Application.Features.Sections.Queries.ListSectionsPaged;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api")]
public sealed class SectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("sectors/{sectorId:guid}/sections")]
    public async Task<IActionResult> CreateSection(Guid sectorId, [FromBody] CreateSectionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateSectionCommand(sectorId, request.Code, request.Name, request.Description),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetSectionById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("sectors/{sectorId:guid}/sections")]
    public async Task<IActionResult> ListSections(
        Guid sectorId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? code = null,
        [FromQuery] string? name = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListSectionsPagedQuery(sectorId, pageNumber, pageSize, orderBy, orderDir, code, name, isActive, includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("sections/{id:guid}")]
    public async Task<IActionResult> GetSectionById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSectionByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("sections/{id:guid}")]
    public async Task<IActionResult> UpdateSection(Guid id, [FromBody] UpdateSectionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateSectionCommand(id, request.SectorId, request.Code, request.Name, request.Description),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("sections/{id:guid}")]
    public async Task<IActionResult> DeactivateSection(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateSectionCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
