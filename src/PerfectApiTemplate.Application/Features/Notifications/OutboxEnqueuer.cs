using PerfectApiTemplate.Application.Abstractions.Notifications;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Application.Features.Notifications;

public sealed class OutboxEnqueuer : IOutboxEnqueuer
{
    private readonly IOutboxRepository _outboxRepository;

    public OutboxEnqueuer(IOutboxRepository outboxRepository)
    {
        _outboxRepository = outboxRepository;
    }

    public async Task EnqueueAsync(string type, string payload, CancellationToken cancellationToken = default)
    {
        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredAtUtc = DateTime.UtcNow,
            Type = type,
            Payload = payload
        };

        await _outboxRepository.AddAsync(message, cancellationToken);
    }
}

