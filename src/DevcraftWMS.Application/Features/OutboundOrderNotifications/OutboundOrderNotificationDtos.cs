namespace DevcraftWMS.Application.Features.OutboundOrderNotifications;

public sealed record OutboundOrderNotificationDto(
    Guid Id,
    Guid OutboundOrderId,
    string EventType,
    int Channel,
    int Status,
    string? ToAddress,
    string? Subject,
    DateTime? SentAtUtc,
    int Attempts,
    string? LastError,
    DateTime CreatedAtUtc);
