using Microsoft.EntityFrameworkCore;
using PerfectApiTemplate.Application.Abstractions.Notifications;
using PerfectApiTemplate.Domain.Entities;
using PerfectApiTemplate.Infrastructure.Persistence;

namespace PerfectApiTemplate.Infrastructure.Messaging;

public sealed class OutboxRepository : IOutboxRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OutboxRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _dbContext.OutboxMessages.Add(message);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OutboxMessages
            .AsNoTracking()
            .Where(m => m.ProcessedAtUtc == null)
            .OrderBy(m => m.OccurredAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkProcessedAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        var tracked = await _dbContext.OutboxMessages.SingleAsync(m => m.Id == message.Id, cancellationToken);
        tracked.ProcessedAtUtc = DateTime.UtcNow;
        tracked.Error = null;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(OutboxMessage message, string error, CancellationToken cancellationToken = default)
    {
        var tracked = await _dbContext.OutboxMessages.SingleAsync(m => m.Id == message.Id, cancellationToken);
        tracked.Error = error;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

