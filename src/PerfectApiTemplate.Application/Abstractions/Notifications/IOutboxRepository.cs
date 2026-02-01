using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Application.Abstractions.Notifications;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken cancellationToken = default);
    Task MarkProcessedAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task MarkFailedAsync(OutboxMessage message, string error, CancellationToken cancellationToken = default);
}

