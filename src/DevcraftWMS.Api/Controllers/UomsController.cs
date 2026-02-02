using MediatR;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Uoms.Commands.CreateUom;
using DevcraftWMS.Application.Features.Uoms.Commands.DeactivateUom;
using DevcraftWMS.Application.Features.Uoms.Commands.UpdateUom;
using DevcraftWMS.Application.Features.Uoms.Queries.GetUomById;
using DevcraftWMS.Application.Features.Uoms.Queries.ListUomsPaged;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class UomsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UomsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("uoms")]
    public async Task<IActionResult> CreateUom([FromBody] CreateUomRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateUomCommand(request.Code, request.Name, request.Type), cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetUomById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("uoms")]
    public async Task<IActionResult> ListUoms(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? code = null,
        [FromQuery] string? name = null,
        [FromQuery] UomType? type = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListUomsPagedQuery(pageNumber, pageSize, orderBy, orderDir, code, name, type, isActive, includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("uoms/{id:guid}")]
    public async Task<IActionResult> GetUomById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUomByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("uoms/{id:guid}")]
    public async Task<IActionResult> UpdateUom(Guid id, [FromBody] UpdateUomRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateUomCommand(id, request.Code, request.Name, request.Type), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("uoms/{id:guid}")]
    public async Task<IActionResult> DeactivateUom(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateUomCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
