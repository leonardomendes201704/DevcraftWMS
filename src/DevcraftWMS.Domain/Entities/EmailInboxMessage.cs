using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Entities;

public enum EmailInboxStatus
{
    [Display(Name = "New")]
    New = 0,
    [Display(Name = "Processed")]
    Processed = 1,
    [Display(Name = "Failed")]
    Failed = 2,
    [Display(Name = "Ignored")]
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


