namespace PerfectApiTemplate.Application.Abstractions.Email;

public sealed record EmailSendRequest(
    string From,
    string To,
    string Subject,
    string Body,
    bool IsHtml);

public sealed record EmailSendResult(
    bool Success,
    string? ProviderMessageId,
    string? Error);

