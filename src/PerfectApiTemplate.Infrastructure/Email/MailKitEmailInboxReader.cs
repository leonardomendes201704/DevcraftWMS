using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PerfectApiTemplate.Application.Abstractions.Email;

namespace PerfectApiTemplate.Infrastructure.Email;

public sealed class MailKitEmailInboxReader : IEmailInboxReader
{
    private readonly EmailInboxOptions _options;
    private readonly ILogger<MailKitEmailInboxReader> _logger;

    public MailKitEmailInboxReader(IOptions<EmailInboxOptions> options, ILogger<MailKitEmailInboxReader> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<EmailInboxItem>> ReadAsync(EmailInboxReadRequest request, CancellationToken cancellationToken = default)
    {
        var maxMessages = request.MaxMessages > 0 ? request.MaxMessages : _options.MaxMessagesPerPoll;
        if (string.Equals(_options.Protocol, "Pop3", StringComparison.OrdinalIgnoreCase))
        {
            return await ReadPop3Async(maxMessages, cancellationToken);
        }

        return await ReadImapAsync(maxMessages, cancellationToken);
    }

    private async Task<IReadOnlyList<EmailInboxItem>> ReadImapAsync(int maxMessages, CancellationToken cancellationToken)
    {
        var items = new List<EmailInboxItem>();
        try
        {
            using var client = new ImapClient();
            await client.ConnectAsync(_options.Host, _options.Port, _options.UseSsl, cancellationToken);
            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            }

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly, cancellationToken);
            var uids = await inbox.SearchAsync(SearchQuery.All, cancellationToken);

            foreach (var uid in uids.TakeLast(maxMessages))
            {
                var message = await inbox.GetMessageAsync(uid, cancellationToken);
                items.Add(MapMessage(message));
            }

            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read IMAP inbox");
        }

        return items;
    }

    private async Task<IReadOnlyList<EmailInboxItem>> ReadPop3Async(int maxMessages, CancellationToken cancellationToken)
    {
        var items = new List<EmailInboxItem>();
        try
        {
            using var client = new Pop3Client();
            await client.ConnectAsync(_options.Host, _options.Port, _options.UseSsl, cancellationToken);
            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            }

            var total = client.Count;
            for (var index = total - 1; index >= 0 && items.Count < maxMessages; index--)
            {
                var message = await client.GetMessageAsync(index, cancellationToken);
                items.Add(MapMessage(message));
            }

            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read POP3 inbox");
        }

        return items;
    }

    private static EmailInboxItem MapMessage(MimeMessage message)
    {
        var body = message.TextBody ?? message.HtmlBody ?? string.Empty;
        var preview = body.Length > 500 ? body[..500] : body;
        var providerId = string.IsNullOrWhiteSpace(message.MessageId)
            ? $"{message.Date.UtcDateTime:O}-{Guid.NewGuid()}"
            : message.MessageId;

        return new EmailInboxItem(
            providerId,
            message.From.ToString(),
            message.To.ToString(),
            message.Subject ?? string.Empty,
            message.Date.UtcDateTime,
            preview);
    }
}

