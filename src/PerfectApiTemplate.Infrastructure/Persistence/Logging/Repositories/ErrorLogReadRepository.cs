using Microsoft.EntityFrameworkCore;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Common.Pagination;
using PerfectApiTemplate.Application.Features.Logs;
using PerfectApiTemplate.Infrastructure.Persistence.Logging;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Repositories;

public sealed class ErrorLogReadRepository : IErrorLogReadRepository
{
    private readonly LogsDbContext _dbContext;

    public ErrorLogReadRepository(LogsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ErrorLogDto>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? source,
        string? eventType,
        string? severity,
        string? exceptionType,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ErrorLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(source))
        {
            query = query.Where(x => x.Source == source);
        }

        if (!string.IsNullOrWhiteSpace(eventType))
        {
            query = query.Where(x => x.EventType != null && x.EventType.Contains(eventType));
        }

        if (!string.IsNullOrWhiteSpace(severity))
        {
            query = query.Where(x => x.Severity == severity);
        }

        if (!string.IsNullOrWhiteSpace(exceptionType))
        {
            query = query.Where(x => x.ExceptionType.Contains(exceptionType));
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
            .Select(x => new ErrorLogDto(
                x.Id,
                x.CreatedAtUtc,
                x.ExceptionType,
                x.Message,
                x.Source,
                x.EventType,
                x.Severity,
                x.StatusCode,
                x.ApiStatusCode,
                x.CorrelationId,
                x.RequestId))
            .ToListAsync(cancellationToken);

        return new PagedResult<ErrorLogDto>(items, total, pageNumber, pageSize, orderBy, orderDir);
    }

    public async Task<ErrorLogDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ErrorLogs.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ErrorLogDetailDto(
                x.Id,
                x.CreatedAtUtc,
                x.ExceptionType,
                x.Message,
                x.Source,
                x.EventType,
                x.Severity,
                x.ClientApp,
                x.ClientEnv,
                x.ClientUrl,
                x.ClientRoute,
                x.ApiMethod,
                x.ApiPath,
                x.ApiStatusCode,
                x.DurationMs,
                x.DetailsJson,
                x.StackTrace,
                x.InnerExceptions,
                x.Method,
                x.Path,
                x.QueryString,
                x.RequestHeaders,
                x.RequestBody,
                x.RequestBodyTruncated,
                x.RequestBodyOriginalLength,
                x.StatusCode,
                x.ApiRequestId,
                x.UserIdText,
                x.TenantId,
                x.CorrelationId,
                x.RequestId,
                x.TraceId,
                x.UserAgent,
                x.ClientIp,
                x.Tags))
            .SingleOrDefaultAsync(cancellationToken);
    }

    private static IQueryable<Infrastructure.Persistence.Logging.Entities.ErrorLog> ApplyOrdering(
        IQueryable<Infrastructure.Persistence.Logging.Entities.ErrorLog> query,
        string orderBy,
        string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "exceptiontype" => asc ? query.OrderBy(x => x.ExceptionType) : query.OrderByDescending(x => x.ExceptionType),
            _ => asc ? query.OrderBy(x => x.CreatedAtUtc) : query.OrderByDescending(x => x.CreatedAtUtc)
        };
    }
}
