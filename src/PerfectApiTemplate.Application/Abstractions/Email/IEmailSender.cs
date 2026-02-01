namespace PerfectApiTemplate.Application.Abstractions.Email;

public interface IEmailSender
{
    Task<EmailSendResult> SendAsync(EmailSendRequest request, CancellationToken cancellationToken = default);
}

