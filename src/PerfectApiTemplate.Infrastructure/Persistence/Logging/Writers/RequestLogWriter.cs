using Microsoft.Extensions.Logging;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Entities;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Queues;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Writers;

public sealed class RequestLogWriter : IRequestLogWriter
{
    private readonly LogQueue<RequestLog> _queue;

    public RequestLogWriter(LogQueue<RequestLog> queue)
    {
        _queue = queue;
    }

    public void Enqueue(RequestLogEntry entry)
    {
        var log = new RequestLog
        {
            Id = Guid.NewGuid(),
            StartedAtUtc = entry.StartedAtUtc,
            FinishedAtUtc = entry.FinishedAtUtc,
            DurationMs = entry.DurationMs,
            Method = entry.Method,
            Scheme = entry.Scheme,
            Host = entry.Host,
            Path = entry.Path,
            QueryString = entry.QueryString,
            StatusCode = entry.StatusCode,
            RequestContentType = entry.RequestContentType,
            RequestContentLength = entry.RequestContentLength,
            ResponseContentType = entry.ResponseContentType,
            ResponseContentLength = entry.ResponseContentLength,
            RequestHeaders = entry.RequestHeaders,
            ResponseHeaders = entry.ResponseHeaders,
            RequestBody = entry.RequestBody,
            ResponseBody = entry.ResponseBody,
            RequestBodyTruncated = entry.RequestBodyTruncated,
            ResponseBodyTruncated = entry.ResponseBodyTruncated,
            RequestBodyOriginalLength = entry.RequestBodyOriginalLength,
            ResponseBodyOriginalLength = entry.ResponseBodyOriginalLength,
            ClientIp = entry.ClientIp,
            UserAgent = entry.UserAgent,
            UserId = entry.UserId,
            TenantId = entry.TenantId,
            CorrelationId = entry.CorrelationId,
            RequestId = entry.RequestId,
            TraceId = entry.TraceId,
            SpanId = entry.SpanId
        };

        _queue.Enqueue(log);
    }
}

