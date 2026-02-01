using MediatR;
using Microsoft.AspNetCore.Mvc;
using PerfectApiTemplate.Api.Contracts;
using PerfectApiTemplate.Api.Extensions;
using PerfectApiTemplate.Application.Features.Emails.Commands.EnqueueEmail;
using PerfectApiTemplate.Application.Features.Emails.Commands.SyncInbox;
using PerfectApiTemplate.Application.Features.Emails.Queries.GetEmailById;
using PerfectApiTemplate.Application.Features.Emails.Queries.ListEmails;
using PerfectApiTemplate.Application.Features.Emails.Queries.ListInboxEmails;

namespace PerfectApiTemplate.Api.Controllers;

[ApiController]
[Route("api/emails")]
public sealed class EmailsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Enqueue([FromBody] EnqueueEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new EnqueueEmailCommand(request.From, request.To, request.Subject, request.Body, request.IsHtml),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEmailByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? cursor = null,
        [FromQuery] string? status = null,
        [FromQuery] string? to = null,
        [FromQuery] string? from = null,
        [FromQuery] string? subject = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListEmailsQuery(pageNumber, pageSize, orderBy, orderDir, cursor, status, to, from, subject, includeInactive),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("inbox")]
    public async Task<IActionResult> ListInbox(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "ReceivedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? cursor = null,
        [FromQuery] string? status = null,
        [FromQuery] string? from = null,
        [FromQuery] string? subject = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListInboxEmailsQuery(pageNumber, pageSize, orderBy, orderDir, cursor, status, from, subject, includeInactive),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("inbox/sync")]
    public async Task<IActionResult> SyncInbox([FromBody] SyncInboxRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SyncInboxCommand(request.MaxMessages), cancellationToken);
        return this.ToActionResult(result);
    }
}

