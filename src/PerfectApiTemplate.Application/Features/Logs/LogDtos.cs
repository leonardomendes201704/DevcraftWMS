namespace PerfectApiTemplate.Application.Features.Logs;

public sealed record RequestLogDto(
    Guid Id,
    DateTime StartedAtUtc,
    long DurationMs,
    string Method,
    string Path,
    int StatusCode,
    string? CorrelationId,
    string? RequestId,
    string? UserAgent);

public sealed record ErrorLogDto(
    Guid Id,
    DateTime CreatedAtUtc,
    string ExceptionType,
    string Message,
    string Source,
    string? EventType,
    string? Severity,
    int? StatusCode,
    int? ApiStatusCode,
    string? CorrelationId,
    string? RequestId);

public sealed record ErrorLogDetailDto(
    Guid Id,
    DateTime CreatedAtUtc,
    string ExceptionType,
    string Message,
    string Source,
    string? EventType,
    string? Severity,
    string? ClientApp,
    string? ClientEnv,
    string? ClientUrl,
    string? ClientRoute,
    string? ApiMethod,
    string? ApiPath,
    int? ApiStatusCode,
    long? DurationMs,
    string? DetailsJson,
    string? StackTrace,
    string? InnerExceptions,
    string Method,
    string Path,
    string? QueryString,
    string? RequestHeaders,
    string? RequestBody,
    bool RequestBodyTruncated,
    long? RequestBodyOriginalLength,
    int? StatusCode,
    string? ApiRequestId,
    string? UserIdText,
    string? TenantId,
    string? CorrelationId,
    string? RequestId,
    string? TraceId,
    string? UserAgent,
    string? ClientIp,
    string? Tags);

public sealed record TransactionLogDto(
    Guid Id,
    DateTime CreatedAtUtc,
    string EntityName,
    string Operation,
    string? EntityId,
    string? CorrelationId,
    string? RequestId);

public sealed record TransactionLogDetailDto(
    Guid Id,
    DateTime CreatedAtUtc,
    string EntityName,
    string Operation,
    string? EntityId,
    string? BeforeJson,
    string? AfterJson,
    string? ChangedProperties,
    string? CorrelationId,
    string? RequestId,
    string? TraceId);
