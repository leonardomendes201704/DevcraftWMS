using Microsoft.EntityFrameworkCore;
using PerfectApiTemplate.Application.Abstractions.Email;
using PerfectApiTemplate.Domain.Entities;
using PerfectApiTemplate.Infrastructure.Persistence;

namespace PerfectApiTemplate.Infrastructure.Email;

public sealed class EmailMessageRepository : IEmailMessageRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmailMessageRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        _dbContext.EmailMessages.Add(message);
        await Task.CompletedTask;
    }

    public async Task<EmailMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailMessages
            .AsNoTracking()
            .Include(x => x.DeliveryAttempts)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<EmailMessage>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? cursor,
        EmailMessageStatus? status,
        string? to,
        string? from,
        string? subject,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.EmailMessages.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(to))
        {
            query = query.Where(x => x.To.Contains(to));
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

    public async Task<IReadOnlyList<EmailMessage>> ListByCreatedAtCursorAsync(
        int pageSize,
        string orderDir,
        DateTime cursorTimeUtc,
        Guid cursorId,
        EmailMessageStatus? status,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.EmailMessages.AsNoTracking();

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
                .Where(x => x.CreatedAtUtc > cursorTimeUtc || (x.CreatedAtUtc == cursorTimeUtc && x.Id.CompareTo(cursorId) > 0))
                .OrderBy(x => x.CreatedAtUtc)
                .ThenBy(x => x.Id);
        }
        else
        {
            query = query
                .Where(x => x.CreatedAtUtc < cursorTimeUtc || (x.CreatedAtUtc == cursorTimeUtc && x.Id.CompareTo(cursorId) < 0))
                .OrderByDescending(x => x.CreatedAtUtc)
                .ThenByDescending(x => x.Id);
        }

        return await query.Take(pageSize).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmailMessage>> GetPendingAsync(int batchSize, int maxAttempts, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailMessages
            .Where(x => x.Status == EmailMessageStatus.Pending || x.Status == EmailMessageStatus.Failed)
            .Where(x => x.AttemptCount < maxAttempts)
            .OrderBy(x => x.CreatedAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAttemptAsync(EmailDeliveryAttempt attempt, CancellationToken cancellationToken = default)
    {
        _dbContext.EmailDeliveryAttempts.Add(attempt);
        await Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<EmailMessage> ApplyOrdering(IQueryable<EmailMessage> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "sentatutc" => asc ? query.OrderBy(x => x.SentAtUtc) : query.OrderByDescending(x => x.SentAtUtc),
            "status" => asc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
            _ => asc ? query.OrderBy(x => x.CreatedAtUtc) : query.OrderByDescending(x => x.CreatedAtUtc)
        };
    }
}

