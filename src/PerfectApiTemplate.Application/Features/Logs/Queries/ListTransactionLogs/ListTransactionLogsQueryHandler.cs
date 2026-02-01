using MediatR;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Logs.Queries.ListTransactionLogs;

public sealed class ListTransactionLogsQueryHandler : IRequestHandler<ListTransactionLogsQuery, RequestResult<PagedResult<TransactionLogDto>>>
{
    private const int MaxPageSize = 200;
    private readonly ITransactionLogReadRepository _repository;

    public ListTransactionLogsQueryHandler(ITransactionLogReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<TransactionLogDto>>> Handle(ListTransactionLogsQuery request, CancellationToken cancellationToken)
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
            request.EntityName,
            request.Operation,
            request.FromUtc,
            request.ToUtc,
            cancellationToken);

        return RequestResult<PagedResult<TransactionLogDto>>.Success(result);
    }
}

