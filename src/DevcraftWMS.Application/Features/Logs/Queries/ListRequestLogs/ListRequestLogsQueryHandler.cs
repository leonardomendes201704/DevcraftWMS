using MediatR;
using DevcraftWMS.Application.Abstractions.Logging;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Logs.Queries.ListRequestLogs;

public sealed class ListRequestLogsQueryHandler : IRequestHandler<ListRequestLogsQuery, RequestResult<PagedResult<RequestLogDto>>>
{
    private readonly IRequestLogReadRepository _repository;

    public ListRequestLogsQueryHandler(IRequestLogReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<RequestLogDto>>> Handle(ListRequestLogsQuery request, CancellationToken cancellationToken)
    {
        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "StartedAtUtc" : request.OrderBy;
        var orderDir = string.Equals(request.OrderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        var result = await _repository.ListAsync(
            request.PageNumber,
            request.PageSize,
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


