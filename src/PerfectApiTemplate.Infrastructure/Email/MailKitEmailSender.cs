using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PerfectApiTemplate.Application.Abstractions.Email;

namespace PerfectApiTemplate.Infrastructure.Email;

public sealed class MailKitEmailSender : IEmailSender
{
    private readonly EmailSmtpOptions _options;
    private readonly ILogger<MailKitEmailSender> _logger;

    public MailKitEmailSender(IOptions<EmailSmtpOptions> options, ILogger<MailKitEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<EmailSendResult> SendAsync(EmailSendRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(request.From));
            message.To.Add(MailboxAddress.Parse(request.To));
            message.Subject = request.Subject;
            message.MessageId = MimeKit.Utils.MimeUtils.GenerateMessageId();

            var builder = new BodyBuilder();
            if (request.IsHtml)
            {
                builder.HtmlBody = request.Body;
            }
            else
            {
                builder.TextBody = request.Body;
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_options.Host, _options.Port, _options.UseSsl, cancellationToken);

            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            return new EmailSendResult(true, message.MessageId, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", request.To);
            return new EmailSendResult(false, null, ex.Message);
        }
    }
}

