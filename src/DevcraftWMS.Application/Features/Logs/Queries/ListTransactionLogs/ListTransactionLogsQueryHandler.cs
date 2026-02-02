using MediatR;
using DevcraftWMS.Application.Abstractions.Logging;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Logs.Queries.ListTransactionLogs;

public sealed class ListTransactionLogsQueryHandler : IRequestHandler<ListTransactionLogsQuery, RequestResult<PagedResult<TransactionLogDto>>>
{
    private readonly ITransactionLogReadRepository _repository;

    public ListTransactionLogsQueryHandler(ITransactionLogReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<TransactionLogDto>>> Handle(ListTransactionLogsQuery request, CancellationToken cancellationToken)
    {
        var orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "CreatedAtUtc" : request.OrderBy;
        var orderDir = string.Equals(request.OrderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        var result = await _repository.ListAsync(
            request.PageNumber,
            request.PageSize,
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


