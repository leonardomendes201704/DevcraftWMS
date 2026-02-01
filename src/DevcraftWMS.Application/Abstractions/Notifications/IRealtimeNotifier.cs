namespace DevcraftWMS.Application.Abstractions.Notifications;

public interface IRealtimeNotifier
{
    Task PublishAsync(string channel, string eventName, object payload, CancellationToken cancellationToken = default);
}

