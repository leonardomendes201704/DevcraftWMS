using Microsoft.Extensions.Logging;
using DevcraftWMS.Application.Abstractions.Notifications;

namespace DevcraftWMS.Infrastructure.Messaging;

public sealed class NoOpNotificationPublisher : INotificationPublisher
{
    private readonly ILogger<NoOpNotificationPublisher> _logger;

    public NoOpNotificationPublisher(ILogger<NoOpNotificationPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(string type, string payload, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Outbox publish no-op. Type: {Type}, PayloadLength: {Length}", type, payload.Length);
        return Task.CompletedTask;
    }
}


