namespace DevcraftWMS.Application.Abstractions.Notifications;

public interface INotificationPublisher
{
    Task PublishAsync(string type, string payload, CancellationToken cancellationToken = default);
}


