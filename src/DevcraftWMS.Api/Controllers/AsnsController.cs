using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Asns.Commands.CreateAsn;
using DevcraftWMS.Application.Features.Asns.Queries.GetAsnById;
using DevcraftWMS.Application.Features.Asns.Queries.ListAsnsPaged;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api")]
public sealed class AsnsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AsnsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("asns")]
    public async Task<IActionResult> CreateAsn([FromBody] CreateAsnRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateAsnCommand(
                request.WarehouseId,
                request.AsnNumber,
                request.DocumentNumber,
                request.SupplierName,
                request.ExpectedArrivalDate,
                request.Notes),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetAsnById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("asns")]
    public async Task<IActionResult> ListAsns(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] string? asnNumber = null,
        [FromQuery] string? supplierName = null,
        [FromQuery] string? documentNumber = null,
        [FromQuery] AsnStatus? status = null,
        [FromQuery] DateOnly? expectedFrom = null,
        [FromQuery] DateOnly? expectedTo = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListAsnsPagedQuery(
                pageNumber,
                pageSize,
                orderBy,
                orderDir,
                warehouseId,
                asnNumber,
                supplierName,
                documentNumber,
                status,
                expectedFrom,
                expectedTo,
                isActive,
                includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("asns/{id:guid}")]
    public async Task<IActionResult> GetAsnById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAsnByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
