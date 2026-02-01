using MediatR;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Logs.Queries.ListTransactionLogs;

public sealed record ListTransactionLogsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? EntityName = null,
    string? Operation = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null) : IRequest<RequestResult<PagedResult<TransactionLogDto>>>;

