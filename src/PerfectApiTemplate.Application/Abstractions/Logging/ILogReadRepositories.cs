using PerfectApiTemplate.Application.Common.Pagination;
using PerfectApiTemplate.Application.Features.Logs;

namespace PerfectApiTemplate.Application.Abstractions.Logging;

public interface IRequestLogReadRepository
{
    Task<PagedResult<RequestLogDto>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        int? statusCode,
        string? pathContains,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default);
}

public interface IErrorLogReadRepository
{
    Task<PagedResult<ErrorLogDto>> ListAsync(
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
        CancellationToken cancellationToken = default);

    Task<ErrorLogDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ITransactionLogReadRepository
{
    Task<PagedResult<TransactionLogDto>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? entityName,
        string? operation,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default);

    Task<TransactionLogDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
