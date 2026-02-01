using MediatR;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Logs.Queries.ListRequestLogs;

public sealed record ListRequestLogsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "StartedAtUtc",
    string OrderDir = "desc",
    int? StatusCode = null,
    string? PathContains = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null) : IRequest<RequestResult<PagedResult<RequestLogDto>>>;

