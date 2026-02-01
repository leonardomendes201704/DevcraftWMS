using MediatR;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Abstractions.Telemetry;
using PerfectApiTemplate.Application.Common.Logging;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Telemetry.Commands.ReportClientEvent;

public sealed class ReportClientEventCommandHandler : IRequestHandler<ReportClientEventCommand, RequestResult<Guid>>
{
    private readonly IErrorLogWriter _writer;
    private readonly TelemetryOptions _options;
    private readonly LoggingOptions _loggingOptions;

    public ReportClientEventCommandHandler(
        IErrorLogWriter writer,
        TelemetryOptions options,
        LoggingOptions loggingOptions)
    {
        _writer = writer;
        _options = options;
        _loggingOptions = loggingOptions;
    }

    public Task<RequestResult<Guid>> Handle(ReportClientEventCommand request, CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return Task.FromResult(RequestResult<Guid>.Failure("telemetry.disabled", "Telemetry is disabled."));
        }

        var createdAt = request.OccurredAtUtc?.UtcDateTime ?? DateTime.UtcNow;
        var maxBytes = _options.MaxPayloadBytes;

        var sanitizedDetails = Truncate(LogMasking.SanitizeJson(
            request.DetailsJson,
            _options.Masking.JsonKeys,
            _loggingOptions.Mask.JsonPaths,
            true), maxBytes);
        var sanitizedMessage = Truncate(request.Message, maxBytes);
        var sanitizedStack = Truncate(request.StackTrace, maxBytes);

        var parsedUserId = Guid.TryParse(request.UserId, out var userId) ? userId : (Guid?)null;

        var entry = new ErrorLogEntry(
            createdAt,
            string.IsNullOrWhiteSpace(request.ExceptionType) ? "client_event" : request.ExceptionType,
            sanitizedMessage ?? string.Empty,
            sanitizedStack,
            null,
            string.IsNullOrWhiteSpace(request.HttpMethod) ? "N/A" : request.HttpMethod,
            string.IsNullOrWhiteSpace(request.ClientUrl) ? "client" : request.ClientUrl,
            null,
            null,
            null,
            false,
            null,
            request.ApiStatusCode,
            parsedUserId,
            request.UserId,
            request.TenantId,
            request.CorrelationId,
            request.RequestId,
            request.ApiRequestId,
            null,
            null,
            null,
            null,
            null,
            "client",
            request.EventType,
            request.Severity,
            request.ClientApp,
            request.ClientEnv,
            request.ClientUrl,
            request.ClientRoute,
            request.ApiMethod,
            request.ApiPath,
            request.ApiStatusCode,
            request.DurationMs,
            sanitizedDetails,
            request.UserAgent,
            request.ClientIp,
            request.Tags);

        _writer.Enqueue(entry);

        return Task.FromResult(RequestResult<Guid>.Success(Guid.NewGuid()));
    }

    private static string? Truncate(string? value, int maxBytes)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        if (value.Length <= maxBytes)
        {
            return value;
        }

        return value[..maxBytes];
    }
}
