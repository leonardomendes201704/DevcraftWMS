using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.CostCenters.Commands.CreateCostCenter;
using DevcraftWMS.Application.Features.CostCenters.Commands.UpdateCostCenter;
using DevcraftWMS.Application.Features.CostCenters.Commands.DeactivateCostCenter;
using DevcraftWMS.Application.Features.CostCenters.Queries.GetCostCenterById;
using DevcraftWMS.Application.Features.CostCenters.Queries.ListCostCentersPaged;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api/cost-centers")]
public sealed class CostCentersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CostCentersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCostCenterRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCostCenterCommand(request.Code, request.Name, request.Description), cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> List(
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
            new ListCostCentersPagedQuery(pageNumber, pageSize, orderBy, orderDir, code, name, isActive, includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCostCenterByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCostCenterRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCostCenterCommand(id, request.Code, request.Name, request.Description), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateCostCenterCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
