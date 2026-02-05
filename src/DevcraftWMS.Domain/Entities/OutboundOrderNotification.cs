using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class OutboundOrderNotification : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid OutboundOrderId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public OutboundOrderNotificationChannel Channel { get; set; }
    public OutboundOrderNotificationStatus Status { get; set; }
    public string? ToAddress { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? Payload { get; set; }
    public string? ExternalId { get; set; }
    public int Attempts { get; set; }
    public DateTime? SentAtUtc { get; set; }
    public string? LastError { get; set; }

    public OutboundOrder? OutboundOrder { get; set; }
}
