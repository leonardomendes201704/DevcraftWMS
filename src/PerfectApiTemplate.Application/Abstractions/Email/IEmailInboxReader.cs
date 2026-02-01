namespace PerfectApiTemplate.Application.Abstractions.Email;

public interface IEmailInboxReader
{
    Task<IReadOnlyList<EmailInboxItem>> ReadAsync(EmailInboxReadRequest request, CancellationToken cancellationToken = default);
}

