namespace DevcraftWMS.Application.Abstractions.Notifications;

public interface IWebhookSender
{
    Task<WebhookSendResult> SendAsync(string url, string payload, CancellationToken cancellationToken = default);
}

public sealed record WebhookSendResult(bool Success, string? Error);
