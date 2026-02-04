using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.QualityInspections;
using DevcraftWMS.Application.Features.QualityInspections.Commands.AddQualityInspectionEvidence;
using DevcraftWMS.Application.Features.QualityInspections.Commands.ApproveQualityInspection;
using DevcraftWMS.Application.Features.QualityInspections.Commands.RejectQualityInspection;
using DevcraftWMS.Application.Features.QualityInspections.Queries.GetQualityInspectionById;
using DevcraftWMS.Application.Features.QualityInspections.Queries.GetQualityInspectionEvidence;
using DevcraftWMS.Application.Features.QualityInspections.Queries.ListQualityInspectionEvidence;
using DevcraftWMS.Application.Features.QualityInspections.Queries.ListQualityInspectionsPaged;
using MediatR;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api/quality-inspections")]
public sealed class QualityInspectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public QualityInspectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListQualityInspectionsPagedQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetQualityInspectionByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] QualityInspectionDecisionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ApproveQualityInspectionCommand(id, request.Notes), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] QualityInspectionDecisionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RejectQualityInspectionCommand(id, request.Notes), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}/evidence")]
    public async Task<IActionResult> ListEvidence(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListQualityInspectionEvidenceQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id:guid}/evidence")]
    public async Task<IActionResult> AddEvidence(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(RequestResult<QualityInspectionEvidenceDto>.Failure("quality.inspection.evidence.required", "Evidence file is required."));
        }

        await using var stream = file.OpenReadStream();
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory, cancellationToken);

        var command = new AddQualityInspectionEvidenceCommand(id, file.FileName, file.ContentType ?? "application/octet-stream", memory.ToArray());
        var result = await _mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}/evidence/{evidenceId:guid}")]
    public async Task<IActionResult> DownloadEvidence(Guid id, Guid evidenceId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetQualityInspectionEvidenceQuery(id, evidenceId), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return this.ToActionResult(result);
        }

        var evidence = result.Value;
        return File(evidence.Content, evidence.ContentType, evidence.FileName);
    }
}
