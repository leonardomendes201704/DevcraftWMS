using MediatR;
using Microsoft.AspNetCore.Mvc;
using PerfectApiTemplate.Api.Extensions;
using PerfectApiTemplate.Application.Features.Logs.Queries.GetErrorLogById;
using PerfectApiTemplate.Application.Features.Logs.Queries.GetTransactionLogById;
using PerfectApiTemplate.Application.Features.Logs.Queries.ListErrorLogs;
using PerfectApiTemplate.Application.Features.Logs.Queries.ListRequestLogs;
using PerfectApiTemplate.Application.Features.Logs.Queries.ListTransactionLogs;

namespace PerfectApiTemplate.Api.Controllers;

[ApiController]
[Route("api/logs")]
public sealed class LogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("requests")]
    public async Task<IActionResult> ListRequests(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "StartedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] int? statusCode = null,
        [FromQuery] string? pathContains = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListRequestLogsQuery(pageNumber, pageSize, orderBy, orderDir, statusCode, pathContains, fromUtc, toUtc),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("errors")]
    public async Task<IActionResult> ListErrors(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? source = null,
        [FromQuery] string? eventType = null,
        [FromQuery] string? severity = null,
        [FromQuery] string? exceptionType = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListErrorLogsQuery(pageNumber, pageSize, orderBy, orderDir, source, eventType, severity, exceptionType, fromUtc, toUtc),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("errors/{id:guid}")]
    public async Task<IActionResult> GetError(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetErrorLogByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> ListTransactions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? entityName = null,
        [FromQuery] string? operation = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListTransactionLogsQuery(pageNumber, pageSize, orderBy, orderDir, entityName, operation, fromUtc, toUtc),
            cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("transactions/{id:guid}")]
    public async Task<IActionResult> GetTransaction(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTransactionLogByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
