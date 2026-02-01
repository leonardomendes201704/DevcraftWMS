using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.Api.Contracts;
using PerfectApiTemplate.Api.Extensions;
using PerfectApiTemplate.Application.Abstractions.Telemetry;
using PerfectApiTemplate.Application.Features.Telemetry.Commands.ReportClientEvent;

namespace PerfectApiTemplate.Api.Controllers;

[ApiController]
[Route("api/telemetry")]
public sealed class TelemetryController : ControllerBase
{
    private const string InternalKeyHeader = "X-Internal-Telemetry-Key";
    private readonly IMediator _mediator;
    private readonly TelemetryOptions _options;

    public TelemetryController(IMediator mediator, IOptions<TelemetryOptions> options)
    {
        _mediator = mediator;
        _options = options.Value;
    }

    [HttpPost("client-events")]
    [AllowAnonymous]
    public async Task<IActionResult> ReportClientEvent([FromBody] ClientTelemetryRequest request, CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return NotFound();
        }

        var hasInternalKey = _options.InternalKeyEnabled
            && Request.Headers.TryGetValue(InternalKeyHeader, out var key)
            && string.Equals(key.ToString(), _options.InternalKey, StringComparison.Ordinal);

        var isAuthenticated = User?.Identity?.IsAuthenticated == true;
        if (_options.RequireAuth && !isAuthenticated && !hasInternalKey)
        {
            return Unauthorized();
        }

        var command = new ReportClientEventCommand(
            request.EventType,
            request.Severity,
            request.ClientApp,
            request.ClientEnv,
            request.ClientUrl,
            request.ClientRoute,
            request.HttpMethod,
            request.CorrelationId,
            request.RequestId,
            request.ApiRequestId,
            request.ApiMethod,
            request.ApiPath,
            request.ApiStatusCode,
            request.DurationMs,
            request.Message,
            request.ExceptionType,
            request.StackTrace,
            request.DetailsJson,
            request.UserId,
            request.TenantId,
            request.UserAgent ?? Request.Headers.UserAgent.ToString(),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            request.Tags,
            request.OccurredAtUtc);

        var result = await _mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }
}
