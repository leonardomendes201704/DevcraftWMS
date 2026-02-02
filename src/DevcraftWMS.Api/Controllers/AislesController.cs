using MediatR;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Aisles.Commands.CreateAisle;
using DevcraftWMS.Application.Features.Aisles.Commands.DeactivateAisle;
using DevcraftWMS.Application.Features.Aisles.Commands.UpdateAisle;
using DevcraftWMS.Application.Features.Aisles.Queries.GetAisleById;
using DevcraftWMS.Application.Features.Aisles.Queries.ListAislesPaged;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class AislesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AislesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("sections/{sectionId:guid}/aisles")]
    public async Task<IActionResult> CreateAisle(Guid sectionId, [FromBody] CreateAisleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateAisleCommand(sectionId, request.Code, request.Name),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetAisleById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("sections/{sectionId:guid}/aisles")]
    public async Task<IActionResult> ListAisles(
        Guid sectionId,
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
            new ListAislesPagedQuery(sectionId, pageNumber, pageSize, orderBy, orderDir, code, name, isActive, includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("aisles/{id:guid}")]
    public async Task<IActionResult> GetAisleById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAisleByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("aisles/{id:guid}")]
    public async Task<IActionResult> UpdateAisle(Guid id, [FromBody] UpdateAisleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateAisleCommand(id, request.SectionId, request.Code, request.Name),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("aisles/{id:guid}")]
    public async Task<IActionResult> DeactivateAisle(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateAisleCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
