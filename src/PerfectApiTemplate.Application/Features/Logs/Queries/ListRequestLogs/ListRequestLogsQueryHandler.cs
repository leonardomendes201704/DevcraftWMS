using MediatR;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Logs.Queries.ListRequestLogs;

public sealed class ListRequestLogsQueryHandler : IRequestHandler<ListRequestLogsQuery, RequestResult<PagedResult<RequestLogDto>>>
{
    private const int MaxPageSize = 200;
    private readonly IRequestLogReadRepository _repository;

    public ListRequestLogsQueryHandler(IRequestLogReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<RequestLogDto>>> Handle(ListRequestLogsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 or > MaxPageSize ? 20 : request.PageSize;
        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "StartedAtUtc" : request.OrderBy;
        var orderDir = string.Equals(request.OrderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        var result = await _repository.ListAsync(
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            request.StatusCode,
            request.PathContains,
            request.FromUtc,
            request.ToUtc,
            cancellationToken);

        return RequestResult<PagedResult<RequestLogDto>>.Success(result);
    }
}

