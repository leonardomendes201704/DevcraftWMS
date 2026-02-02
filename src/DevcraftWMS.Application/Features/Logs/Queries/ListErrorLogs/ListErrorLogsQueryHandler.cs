using MediatR;
using DevcraftWMS.Application.Abstractions.Logging;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Logs.Queries.ListErrorLogs;

public sealed class ListErrorLogsQueryHandler : IRequestHandler<ListErrorLogsQuery, RequestResult<PagedResult<ErrorLogDto>>>
{
    private readonly IErrorLogReadRepository _repository;

    public ListErrorLogsQueryHandler(IErrorLogReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<ErrorLogDto>>> Handle(ListErrorLogsQuery request, CancellationToken cancellationToken)
    {
        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "CreatedAtUtc" : request.OrderBy;
        var orderDir = string.Equals(request.OrderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        var result = await _repository.ListAsync(
            request.PageNumber,
            request.PageSize,
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

