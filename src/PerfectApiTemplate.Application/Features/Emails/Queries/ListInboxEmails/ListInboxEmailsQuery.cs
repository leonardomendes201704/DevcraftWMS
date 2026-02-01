using MediatR;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Emails.Queries.ListInboxEmails;

public sealed record ListInboxEmailsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "ReceivedAtUtc",
    string OrderDir = "desc",
    string? Cursor = null,
    string? Status = null,
    string? From = null,
    string? Subject = null,
    bool IncludeInactive = false) : IRequest<RequestResult<CursorPaginationResult<EmailInboxMessageDto>>>;

