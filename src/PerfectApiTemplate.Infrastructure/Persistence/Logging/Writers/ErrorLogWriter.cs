using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Entities;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Queues;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Writers;

public sealed class ErrorLogWriter : IErrorLogWriter
{
    private readonly LogQueue<ErrorLog> _queue;

    public ErrorLogWriter(LogQueue<ErrorLog> queue)
    {
        _queue = queue;
    }

    public void Enqueue(ErrorLogEntry entry)
    {
        var log = new ErrorLog
        {
            Id = Guid.NewGuid(),
            CreatedAtUtc = entry.CreatedAtUtc,
            ExceptionType = entry.ExceptionType,
            Message = entry.Message,
            StackTrace = entry.StackTrace,
            InnerExceptions = entry.InnerExceptions,
            Method = entry.Method,
            Path = entry.Path,
            QueryString = entry.QueryString,
            RequestHeaders = entry.RequestHeaders,
            RequestBody = entry.RequestBody,
            RequestBodyTruncated = entry.RequestBodyTruncated,
            RequestBodyOriginalLength = entry.RequestBodyOriginalLength,
            StatusCode = entry.StatusCode,
            UserId = entry.UserId,
            UserIdText = entry.UserIdText,
            TenantId = entry.TenantId,
            CorrelationId = entry.CorrelationId,
            RequestId = entry.RequestId,
            ApiRequestId = entry.ApiRequestId,
            TraceId = entry.TraceId,
            SpanId = entry.SpanId,
            EnvironmentName = entry.EnvironmentName,
            MachineName = entry.MachineName,
            AssemblyVersion = entry.AssemblyVersion,
            Source = entry.Source,
            EventType = entry.EventType,
            Severity = entry.Severity,
            ClientApp = entry.ClientApp,
            ClientEnv = entry.ClientEnv,
            ClientUrl = entry.ClientUrl,
            ClientRoute = entry.ClientRoute,
            ApiMethod = entry.ApiMethod,
            ApiPath = entry.ApiPath,
            ApiStatusCode = entry.ApiStatusCode,
            DurationMs = entry.DurationMs,
            DetailsJson = entry.DetailsJson,
            UserAgent = entry.UserAgent,
            ClientIp = entry.ClientIp,
            Tags = entry.Tags
        };

        _queue.Enqueue(log);
    }
}
