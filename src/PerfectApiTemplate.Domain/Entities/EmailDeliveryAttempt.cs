namespace PerfectApiTemplate.Domain.Entities;

public sealed class EmailDeliveryAttempt : AuditableEntity
{
    public Guid EmailMessageId { get; set; }
    public EmailMessage EmailMessage { get; set; } = null!;
    public DateTime AttemptedAtUtc { get; set; }
    public bool Succeeded { get; set; }
    public string? Error { get; set; }
}

