namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Entities;

public sealed class TransactionLog
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }
    public string? ChangedProperties { get; set; }
    public long DurationMs { get; set; }
    public string OperationId { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? TenantId { get; set; }
    public string? CorrelationId { get; set; }
    public string? RequestId { get; set; }
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }
}

