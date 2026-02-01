namespace PerfectApiTemplate.Api.Contracts;

public sealed record EnqueueEmailRequest(
    string? From,
    string To,
    string Subject,
    string Body,
    bool IsHtml);

public sealed record SyncInboxRequest(int MaxMessages);

