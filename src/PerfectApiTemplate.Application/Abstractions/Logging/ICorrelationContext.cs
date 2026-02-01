namespace PerfectApiTemplate.Application.Abstractions.Logging;

public interface ICorrelationContext
{
    string? CorrelationId { get; }
    string? RequestId { get; }
    string? TraceId { get; }
    string? SpanId { get; }
    string? TenantId { get; }
    Guid? UserId { get; }
}

