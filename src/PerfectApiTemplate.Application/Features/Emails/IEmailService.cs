using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Features.Emails;

public interface IEmailService
{
    Task<RequestResult<EmailMessageDto>> EnqueueAsync(
        string? from,
        string to,
        string subject,
        string body,
        bool isHtml,
        CancellationToken cancellationToken);

    Task<RequestResult<EmailMessageDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<RequestResult<CursorPaginationResult<EmailMessageDto>>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? cursor,
        string? status,
        string? to,
        string? from,
        string? subject,
        bool includeInactive,
        CancellationToken cancellationToken);

    Task<RequestResult<CursorPaginationResult<EmailInboxMessageDto>>> ListInboxAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? cursor,
        string? status,
        string? from,
        string? subject,
        bool includeInactive,
        CancellationToken cancellationToken);

    Task<RequestResult<int>> SyncInboxAsync(int maxMessages, CancellationToken cancellationToken);
}

