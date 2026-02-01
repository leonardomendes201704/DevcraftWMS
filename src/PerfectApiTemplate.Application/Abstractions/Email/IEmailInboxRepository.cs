using PerfectApiTemplate.Application.Common.Pagination;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Application.Abstractions.Email;

public interface IEmailInboxRepository
{
    Task AddAsync(EmailInboxMessage message, CancellationToken cancellationToken = default);
    Task<bool> ExistsByProviderMessageIdAsync(string providerMessageId, CancellationToken cancellationToken = default);
    Task<EmailInboxMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailInboxMessage>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? cursor,
        EmailInboxStatus? status,
        string? from,
        string? subject,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailInboxMessage>> ListByReceivedAtCursorAsync(
        int pageSize,
        string orderDir,
        DateTime cursorTimeUtc,
        Guid cursorId,
        EmailInboxStatus? status,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
