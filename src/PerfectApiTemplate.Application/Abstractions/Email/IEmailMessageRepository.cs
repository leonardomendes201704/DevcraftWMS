using PerfectApiTemplate.Domain.Entities;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Application.Abstractions.Email;

public interface IEmailMessageRepository
{
    Task AddAsync(EmailMessage message, CancellationToken cancellationToken = default);
    Task<EmailMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailMessage>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? cursor,
        EmailMessageStatus? status,
        string? to,
        string? from,
        string? subject,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailMessage>> ListByCreatedAtCursorAsync(
        int pageSize,
        string orderDir,
        DateTime cursorTimeUtc,
        Guid cursorId,
        EmailMessageStatus? status,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailMessage>> GetPendingAsync(int batchSize, int maxAttempts, CancellationToken cancellationToken = default);
    Task AddAttemptAsync(EmailDeliveryAttempt attempt, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
