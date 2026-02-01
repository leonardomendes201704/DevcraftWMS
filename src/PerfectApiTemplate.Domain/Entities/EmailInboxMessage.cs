namespace PerfectApiTemplate.Domain.Entities;

public enum EmailInboxStatus
{
    New = 0,
    Processed = 1,
    Failed = 2,
    Ignored = 3
}

public sealed class EmailInboxMessage : AuditableEntity
{
    public string ProviderMessageId { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateTime ReceivedAtUtc { get; set; }
    public string? BodyPreview { get; set; }
    public EmailInboxStatus Status { get; set; } = EmailInboxStatus.New;
    public int ProcessingAttempts { get; set; }
    public DateTime? LastProcessedAtUtc { get; set; }
    public string? LastError { get; set; }
}

