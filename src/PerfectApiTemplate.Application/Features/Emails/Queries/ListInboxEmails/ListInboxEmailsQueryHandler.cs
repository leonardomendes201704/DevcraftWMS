using MediatR;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Emails.Queries.ListInboxEmails;

public sealed class ListInboxEmailsQueryHandler : IRequestHandler<ListInboxEmailsQuery, RequestResult<CursorPaginationResult<EmailInboxMessageDto>>>
{
    private readonly IEmailService _emailService;

    public ListInboxEmailsQueryHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task<RequestResult<CursorPaginationResult<EmailInboxMessageDto>>> Handle(ListInboxEmailsQuery request, CancellationToken cancellationToken)
        => _emailService.ListInboxAsync(
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            request.Cursor,
            request.Status,
            request.From,
            request.Subject,
            request.IncludeInactive,
            cancellationToken);
}

