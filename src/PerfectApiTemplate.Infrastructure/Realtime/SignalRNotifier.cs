using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PerfectApiTemplate.Application.Abstractions.Notifications;

namespace PerfectApiTemplate.Infrastructure.Realtime;

public sealed class SignalRNotifier : IRealtimeNotifier
{
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly ILogger<SignalRNotifier> _logger;

    public SignalRNotifier(IHubContext<NotificationsHub> hubContext, ILogger<SignalRNotifier> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task PublishAsync(string channel, string eventName, object payload, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.Group(channel).SendAsync(eventName, payload, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish SignalR event {EventName} to {Channel}", eventName, channel);
        }
    }
}
