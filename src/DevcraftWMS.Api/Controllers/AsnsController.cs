using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Asns.Commands.AddAsnItem;
using DevcraftWMS.Application.Features.Asns.Commands.CreateAsn;
using DevcraftWMS.Application.Features.Asns.Commands.AddAsnAttachment;
using DevcraftWMS.Application.Features.Asns.Commands.SubmitAsn;
using DevcraftWMS.Application.Features.Asns.Commands.ApproveAsn;
using DevcraftWMS.Application.Features.Asns.Commands.ConvertAsn;
using DevcraftWMS.Application.Features.Asns.Commands.CancelAsn;
using DevcraftWMS.Application.Features.Asns.Queries.GetAsnById;
using DevcraftWMS.Application.Features.Asns.Queries.DownloadAsnAttachment;
using DevcraftWMS.Application.Features.Asns.Queries.ListAsnAttachments;
using DevcraftWMS.Application.Features.Asns.Queries.ListAsnsPaged;
using DevcraftWMS.Application.Features.Asns.Queries.ListAsnItems;
using DevcraftWMS.Application.Features.Asns.Queries.ListAsnStatusEvents;
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

    [HttpGet("asns/{id:guid}/attachments")]
    public async Task<IActionResult> ListAttachments(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListAsnAttachmentsQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("asns/{id:guid}/attachments/{attachmentId:guid}/download")]
    public async Task<IActionResult> DownloadAttachment(Guid id, Guid attachmentId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DownloadAsnAttachmentQuery(id, attachmentId), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return this.ToActionResult(result);
        }

        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HttpPost("asns/{id:guid}/attachments")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> AddAttachment(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "File is required." });
        }

        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);

        var result = await _mediator.Send(
            new AddAsnAttachmentCommand(
                id,
                file.FileName,
                file.ContentType ?? "application/octet-stream",
                file.Length,
                stream.ToArray()),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("asns/{id:guid}/items")]
    public async Task<IActionResult> ListItems(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListAsnItemsQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("asns/{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, [FromBody] AddAsnItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddAsnItemCommand(
                id,
                request.ProductId,
                request.UomId,
                request.Quantity,
                request.LotCode,
                request.ExpirationDate),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("asns/{id:guid}/status-events")]
    public async Task<IActionResult> ListStatusEvents(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListAsnStatusEventsQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("asns/{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id, [FromBody] AsnStatusChangeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SubmitAsnCommand(id, request.Notes), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("asns/{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] AsnStatusChangeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ApproveAsnCommand(id, request.Notes), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("asns/{id:guid}/convert")]
    public async Task<IActionResult> Convert(Guid id, [FromBody] AsnStatusChangeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ConvertAsnCommand(id, request.Notes), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("asns/{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] AsnStatusChangeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelAsnCommand(id, request.Notes), cancellationToken);
        return this.ToActionResult(result);
    }
}
