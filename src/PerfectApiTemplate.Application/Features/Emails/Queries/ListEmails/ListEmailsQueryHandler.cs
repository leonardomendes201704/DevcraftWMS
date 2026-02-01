using MediatR;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Emails.Queries.ListEmails;

public sealed class ListEmailsQueryHandler : IRequestHandler<ListEmailsQuery, RequestResult<CursorPaginationResult<EmailMessageDto>>>
{
    private readonly IEmailService _emailService;

    public ListEmailsQueryHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task<RequestResult<CursorPaginationResult<EmailMessageDto>>> Handle(ListEmailsQuery request, CancellationToken cancellationToken)
        => _emailService.ListAsync(
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            request.Cursor,
            request.Status,
            request.To,
            request.From,
            request.Subject,
            request.IncludeInactive,
            cancellationToken);
}

