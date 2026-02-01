namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Entities;

public sealed class RequestLog
{
    public Guid Id { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime FinishedAtUtc { get; set; }
    public long DurationMs { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Scheme { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public int StatusCode { get; set; }
    public string? RequestContentType { get; set; }
    public long? RequestContentLength { get; set; }
    public string? ResponseContentType { get; set; }
    public long? ResponseContentLength { get; set; }
    public string? RequestHeaders { get; set; }
    public string? ResponseHeaders { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public bool RequestBodyTruncated { get; set; }
    public bool ResponseBodyTruncated { get; set; }
    public long? RequestBodyOriginalLength { get; set; }
    public long? ResponseBodyOriginalLength { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
    public Guid? UserId { get; set; }
    public string? TenantId { get; set; }
    public string? CorrelationId { get; set; }
    public string? RequestId { get; set; }
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }
}

