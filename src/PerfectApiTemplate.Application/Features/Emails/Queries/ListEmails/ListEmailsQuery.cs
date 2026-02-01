using MediatR;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Emails.Queries.ListEmails;

public sealed record ListEmailsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Cursor = null,
    string? Status = null,
    string? To = null,
    string? From = null,
    string? Subject = null,
    bool IncludeInactive = false) : IRequest<RequestResult<CursorPaginationResult<EmailMessageDto>>>;

