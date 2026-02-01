using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Application.Features.Emails;

public sealed record EmailMessageDto(
    Guid Id,
    string From,
    string To,
    string Subject,
    EmailMessageStatus Status,
    int AttemptCount,
    DateTime CreatedAtUtc,
    DateTime? SentAtUtc,
    string? LastError);

public sealed record EmailInboxMessageDto(
    Guid Id,
    string ProviderMessageId,
    string From,
    string To,
    string Subject,
    DateTime ReceivedAtUtc,
    EmailInboxStatus Status,
    int ProcessingAttempts,
    DateTime CreatedAtUtc,
    string? LastError);

