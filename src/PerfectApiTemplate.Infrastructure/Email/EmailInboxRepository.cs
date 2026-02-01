using Microsoft.EntityFrameworkCore;
using PerfectApiTemplate.Application.Abstractions.Email;
using PerfectApiTemplate.Domain.Entities;
using PerfectApiTemplate.Infrastructure.Persistence;

namespace PerfectApiTemplate.Infrastructure.Email;

public sealed class EmailInboxRepository : IEmailInboxRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmailInboxRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(EmailInboxMessage message, CancellationToken cancellationToken = default)
    {
        _dbContext.EmailInboxMessages.Add(message);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsByProviderMessageIdAsync(string providerMessageId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailInboxMessages
            .AsNoTracking()
            .AnyAsync(x => x.ProviderMessageId == providerMessageId, cancellationToken);
    }

    public async Task<EmailInboxMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailInboxMessages
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<EmailInboxMessage>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? cursor,
        EmailInboxStatus? status,
        string? from,
        string? subject,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.EmailInboxMessages.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(from))
        {
            query = query.Where(x => x.From.Contains(from));
        }

        if (!string.IsNullOrWhiteSpace(subject))
        {
            query = query.Where(x => x.Subject.Contains(subject));
        }

        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmailInboxMessage>> ListByReceivedAtCursorAsync(
        int pageSize,
        string orderDir,
        DateTime cursorTimeUtc,
        Guid cursorId,
        EmailInboxStatus? status,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.EmailInboxMessages.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status);
        }

        if (string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase))
        {
            query = query
                .Where(x => x.ReceivedAtUtc > cursorTimeUtc || (x.ReceivedAtUtc == cursorTimeUtc && x.Id.CompareTo(cursorId) > 0))
                .OrderBy(x => x.ReceivedAtUtc)
                .ThenBy(x => x.Id);
        }
        else
        {
            query = query
                .Where(x => x.ReceivedAtUtc < cursorTimeUtc || (x.ReceivedAtUtc == cursorTimeUtc && x.Id.CompareTo(cursorId) < 0))
                .OrderByDescending(x => x.ReceivedAtUtc)
                .ThenByDescending(x => x.Id);
        }

        return await query.Take(pageSize).ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<EmailInboxMessage> ApplyOrdering(IQueryable<EmailInboxMessage> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "receivedatutc" => asc ? query.OrderBy(x => x.ReceivedAtUtc) : query.OrderByDescending(x => x.ReceivedAtUtc),
            "status" => asc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
            _ => asc ? query.OrderBy(x => x.CreatedAtUtc) : query.OrderByDescending(x => x.CreatedAtUtc)
        };
    }
}

