namespace PerfectApiTemplate.Application.Abstractions.Notifications;

public interface IOutboxEnqueuer
{
    Task EnqueueAsync(string type, string payload, CancellationToken cancellationToken = default);
}

