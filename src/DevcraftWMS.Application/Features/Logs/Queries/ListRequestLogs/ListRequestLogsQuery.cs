using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Logs.Queries.ListRequestLogs;

public sealed record ListRequestLogsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "StartedAtUtc",
    string OrderDir = "desc",
    int? StatusCode = null,
    string? PathContains = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null) : IRequest<RequestResult<PagedResult<RequestLogDto>>>;


