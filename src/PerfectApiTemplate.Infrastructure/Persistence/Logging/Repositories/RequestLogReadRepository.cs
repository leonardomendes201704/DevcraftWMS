using Microsoft.EntityFrameworkCore;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Common.Pagination;
using PerfectApiTemplate.Application.Features.Logs;
using PerfectApiTemplate.Infrastructure.Persistence.Logging;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Repositories;

public sealed class RequestLogReadRepository : IRequestLogReadRepository
{
    private readonly LogsDbContext _dbContext;

    public RequestLogReadRepository(LogsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<RequestLogDto>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        int? statusCode,
        string? pathContains,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.RequestLogs.AsNoTracking();

        if (statusCode.HasValue)
        {
            query = query.Where(x => x.StatusCode == statusCode.Value);
        }

        if (!string.IsNullOrWhiteSpace(pathContains))
        {
            query = query.Where(x => x.Path.Contains(pathContains));
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(x => x.StartedAtUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(x => x.StartedAtUtc <= toUtc.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        query = ApplyOrdering(query, orderBy, orderDir);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new RequestLogDto(
                x.Id,
                x.StartedAtUtc,
                x.DurationMs,
                x.Method,
                x.Path,
                x.StatusCode,
                x.CorrelationId,
                x.RequestId,
                x.UserAgent))
            .ToListAsync(cancellationToken);

        return new PagedResult<RequestLogDto>(items, total, pageNumber, pageSize, orderBy, orderDir);
    }

    private static IQueryable<Infrastructure.Persistence.Logging.Entities.RequestLog> ApplyOrdering(
        IQueryable<Infrastructure.Persistence.Logging.Entities.RequestLog> query,
        string orderBy,
        string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "statuscode" => asc ? query.OrderBy(x => x.StatusCode) : query.OrderByDescending(x => x.StatusCode),
            "path" => asc ? query.OrderBy(x => x.Path) : query.OrderByDescending(x => x.Path),
            "durationms" => asc ? query.OrderBy(x => x.DurationMs) : query.OrderByDescending(x => x.DurationMs),
            _ => asc ? query.OrderBy(x => x.StartedAtUtc) : query.OrderByDescending(x => x.StartedAtUtc)
        };
    }
}

