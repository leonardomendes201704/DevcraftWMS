namespace DevcraftWMS.Application.Features.InboundOrderNotifications;

public sealed class InboundOrderNotificationOptions
{
    public bool EmailEnabled { get; set; } = true;
    public bool WebhookEnabled { get; set; }
    public bool PortalEnabled { get; set; } = true;
    public string? WebhookUrl { get; set; }
    public int WebhookTimeoutSeconds { get; set; } = 10;
}
