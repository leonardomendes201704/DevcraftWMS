using System.Net.Http.Json;
using DevcraftWMS.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace DevcraftWMS.Infrastructure.Notifications;

public sealed class WebhookSender : IWebhookSender
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhookSender> _logger;

    public WebhookSender(HttpClient httpClient, ILogger<WebhookSender> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WebhookSendResult> SendAsync(string url, string payload, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return new WebhookSendResult(false, "Webhook URL is required.");
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json")
            };

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                return new WebhookSendResult(false, $"Webhook failed ({(int)response.StatusCode}). {body}");
            }

            return new WebhookSendResult(true, null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Webhook delivery failed.");
            return new WebhookSendResult(false, "Webhook delivery failed.");
        }
    }
}
