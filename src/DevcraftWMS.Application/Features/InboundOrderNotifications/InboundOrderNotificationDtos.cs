namespace DevcraftWMS.Application.Features.InboundOrderNotifications;

public sealed record InboundOrderNotificationDto(
    Guid Id,
    Guid InboundOrderId,
    string EventType,
    int Channel,
    int Status,
    string? ToAddress,
    string? Subject,
    DateTime? SentAtUtc,
    int Attempts,
    string? LastError,
    DateTime CreatedAtUtc);
