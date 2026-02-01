namespace PerfectApiTemplate.Domain.Entities;

public sealed class OutboxMessage : AuditableEntity
{
    public DateTime OccurredAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string? Error { get; set; }
}

