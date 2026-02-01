using MediatR;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Logs.Queries.ListErrorLogs;

public sealed class ListErrorLogsQueryHandler : IRequestHandler<ListErrorLogsQuery, RequestResult<PagedResult<ErrorLogDto>>>
{
    private const int MaxPageSize = 200;
    private readonly IErrorLogReadRepository _repository;

    public ListErrorLogsQueryHandler(IErrorLogReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<ErrorLogDto>>> Handle(ListErrorLogsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 or > MaxPageSize ? 20 : request.PageSize;
        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "CreatedAtUtc" : request.OrderBy;
        var orderDir = string.Equals(request.OrderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        var result = await _repository.ListAsync(
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            request.Source,
            request.EventType,
            request.Severity,
            request.ExceptionType,
            request.FromUtc,
            request.ToUtc,
            cancellationToken);

        return RequestResult<PagedResult<ErrorLogDto>>.Success(result);
    }
}
