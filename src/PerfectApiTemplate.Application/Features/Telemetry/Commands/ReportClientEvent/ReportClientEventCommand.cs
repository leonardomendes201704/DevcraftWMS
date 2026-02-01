using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Telemetry.Commands.ReportClientEvent;

public sealed record ReportClientEventCommand(
    string EventType,
    string Severity,
    string ClientApp,
    string ClientEnv,
    string ClientUrl,
    string ClientRoute,
    string HttpMethod,
    string CorrelationId,
    string? RequestId,
    string? ApiRequestId,
    string? ApiMethod,
    string? ApiPath,
    int? ApiStatusCode,
    long? DurationMs,
    string Message,
    string? ExceptionType,
    string? StackTrace,
    string? DetailsJson,
    string? UserId,
    string? TenantId,
    string? UserAgent,
    string? ClientIp,
    string? Tags,
    DateTimeOffset? OccurredAtUtc) : IRequest<RequestResult<Guid>>;
