namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Entities;

public sealed class ErrorLog
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string ExceptionType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? InnerExceptions { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public string? RequestHeaders { get; set; }
    public string? RequestBody { get; set; }
    public bool RequestBodyTruncated { get; set; }
    public long? RequestBodyOriginalLength { get; set; }
    public int? StatusCode { get; set; }
    public Guid? UserId { get; set; }
    public string? UserIdText { get; set; }
    public string? TenantId { get; set; }
    public string? CorrelationId { get; set; }
    public string? RequestId { get; set; }
    public string? ApiRequestId { get; set; }
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }
    public string? EnvironmentName { get; set; }
    public string? MachineName { get; set; }
    public string? AssemblyVersion { get; set; }
    public string Source { get; set; } = "server";
    public string? EventType { get; set; }
    public string? Severity { get; set; }
    public string? ClientApp { get; set; }
    public string? ClientEnv { get; set; }
    public string? ClientUrl { get; set; }
    public string? ClientRoute { get; set; }
    public string? ApiMethod { get; set; }
    public string? ApiPath { get; set; }
    public int? ApiStatusCode { get; set; }
    public long? DurationMs { get; set; }
    public string? DetailsJson { get; set; }
    public string? UserAgent { get; set; }
    public string? ClientIp { get; set; }
    public string? Tags { get; set; }
}
