namespace PerfectApiTemplate.Application.Abstractions.Email;

public sealed record EmailInboxReadRequest(int MaxMessages);

public sealed record EmailInboxItem(
    string ProviderMessageId,
    string From,
    string To,
    string Subject,
    DateTime ReceivedAtUtc,
    string? BodyPreview);

