namespace PerfectApiTemplate.Domain.Entities;

public enum EmailMessageStatus
{
    Pending = 0,
    Sending = 1,
    Sent = 2,
    Failed = 3
}

public sealed class EmailMessage : AuditableEntity
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; }
    public EmailMessageStatus Status { get; set; } = EmailMessageStatus.Pending;
    public int AttemptCount { get; set; }
    public DateTime? LastAttemptAtUtc { get; set; }
    public string? LastError { get; set; }
    public DateTime? SentAtUtc { get; set; }
    public string? ProviderMessageId { get; set; }
    public ICollection<EmailDeliveryAttempt> DeliveryAttempts { get; set; } = new List<EmailDeliveryAttempt>();
}

