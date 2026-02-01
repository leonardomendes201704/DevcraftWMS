using Microsoft.EntityFrameworkCore;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Common.Pagination;
using PerfectApiTemplate.Application.Features.Logs;
using PerfectApiTemplate.Infrastructure.Persistence.Logging;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Repositories;

public sealed class TransactionLogReadRepository : ITransactionLogReadRepository
{
    private readonly LogsDbContext _dbContext;

    public TransactionLogReadRepository(LogsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<TransactionLogDto>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? entityName,
        string? operation,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.TransactionLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            query = query.Where(x => x.EntityName.Contains(entityName));
        }

        if (!string.IsNullOrWhiteSpace(operation))
        {
            query = query.Where(x => x.Operation == operation);
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(x => x.CreatedAtUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(x => x.CreatedAtUtc <= toUtc.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        query = ApplyOrdering(query, orderBy, orderDir);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TransactionLogDto(
                x.Id,
                x.CreatedAtUtc,
                x.EntityName,
                x.Operation,
                x.EntityId,
                x.CorrelationId,
                x.RequestId))
            .ToListAsync(cancellationToken);

        return new PagedResult<TransactionLogDto>(items, total, pageNumber, pageSize, orderBy, orderDir);
    }

    public async Task<TransactionLogDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TransactionLogs.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new TransactionLogDetailDto(
                x.Id,
                x.CreatedAtUtc,
                x.EntityName,
                x.Operation,
                x.EntityId,
                x.BeforeJson,
                x.AfterJson,
                x.ChangedProperties,
                x.CorrelationId,
                x.RequestId,
                x.TraceId))
            .SingleOrDefaultAsync(cancellationToken);
    }

    private static IQueryable<Infrastructure.Persistence.Logging.Entities.TransactionLog> ApplyOrdering(
        IQueryable<Infrastructure.Persistence.Logging.Entities.TransactionLog> query,
        string orderBy,
        string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "entityname" => asc ? query.OrderBy(x => x.EntityName) : query.OrderByDescending(x => x.EntityName),
            "operation" => asc ? query.OrderBy(x => x.Operation) : query.OrderByDescending(x => x.Operation),
            _ => asc ? query.OrderBy(x => x.CreatedAtUtc) : query.OrderByDescending(x => x.CreatedAtUtc)
        };
    }
}

