using PerfectApiTemplate.Application.Abstractions.Email;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Common.Pagination;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Application.Features.Emails;

public sealed class EmailService : IEmailService
{
    private const int MaxPageSize = 100;
    private readonly IEmailMessageRepository _messageRepository;
    private readonly IEmailInboxRepository _inboxRepository;
    private readonly IEmailInboxReader _inboxReader;
    private readonly IEmailDefaults _defaults;

    public EmailService(
        IEmailMessageRepository messageRepository,
        IEmailInboxRepository inboxRepository,
        IEmailInboxReader inboxReader,
        IEmailDefaults defaults)
    {
        _messageRepository = messageRepository;
        _inboxRepository = inboxRepository;
        _inboxReader = inboxReader;
        _defaults = defaults;
    }

    public async Task<RequestResult<EmailMessageDto>> EnqueueAsync(
        string? from,
        string to,
        string subject,
        string body,
        bool isHtml,
        CancellationToken cancellationToken)
    {
        var message = new EmailMessage
        {
            From = string.IsNullOrWhiteSpace(from) ? _defaults.DefaultFrom : from,
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml,
            Status = EmailMessageStatus.Pending
        };

        await _messageRepository.AddAsync(message, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);

        return RequestResult<EmailMessageDto>.Success(MapMessage(message));
    }

    public async Task<RequestResult<EmailMessageDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(id, cancellationToken);
        if (message is null)
        {
            return RequestResult<EmailMessageDto>.Failure("emails.message.not_found", "Email message was not found.");
        }

        return RequestResult<EmailMessageDto>.Success(MapMessage(message));
    }

    public async Task<RequestResult<CursorPaginationResult<EmailMessageDto>>> ListAsync(
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
        CancellationToken cancellationToken)
    {
        var parsedStatus = ParseStatus(status, out var statusFilter, out var statusError);
        if (!parsedStatus)
        {
            return RequestResult<CursorPaginationResult<EmailMessageDto>>.Failure("emails.message.invalid_status", statusError);
        }

        var normalizedPageNumber = pageNumber < 1 ? 1 : pageNumber;
        var normalizedPageSize = pageSize is < 1 or > MaxPageSize ? 20 : pageSize;
        var normalizedOrderBy = string.IsNullOrWhiteSpace(orderBy) ? "CreatedAtUtc" : orderBy;
        var normalizedOrderDir = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        if (CursorToken.TryParse(cursor, out var token) &&
            normalizedOrderBy.Equals("CreatedAtUtc", StringComparison.OrdinalIgnoreCase))
        {
            if (token.Parts.Count >= 2 &&
                DateTime.TryParse(token.Parts[0], out var cursorTime) &&
                Guid.TryParse(token.Parts[1], out var cursorId))
            {
                var items = await _messageRepository.ListByCreatedAtCursorAsync(
                    normalizedPageSize,
                    normalizedOrderDir,
                    cursorTime,
                    cursorId,
                    statusFilter,
                    includeInactive,
                    cancellationToken);

                var dtos = items.Select(MapMessage).ToList();
                var nextCursor = dtos.Count == 0
                    ? null
                    : CursorToken.Build(dtos[^1].CreatedAtUtc.ToString("O"), dtos[^1].Id);

                return RequestResult<CursorPaginationResult<EmailMessageDto>>.Success(
                    new CursorPaginationResult<EmailMessageDto>(dtos, nextCursor, normalizedPageSize, normalizedOrderBy, normalizedOrderDir));
            }
        }

        var list = await _messageRepository.ListAsync(
            normalizedPageNumber,
            normalizedPageSize,
            normalizedOrderBy,
            normalizedOrderDir,
            cursor,
            statusFilter,
            to,
            from,
            subject,
            includeInactive,
            cancellationToken);

        var fallbackDtos = list.Select(MapMessage).ToList();
        return RequestResult<CursorPaginationResult<EmailMessageDto>>.Success(
            new CursorPaginationResult<EmailMessageDto>(fallbackDtos, null, normalizedPageSize, normalizedOrderBy, normalizedOrderDir));
    }

    public async Task<RequestResult<CursorPaginationResult<EmailInboxMessageDto>>> ListInboxAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? cursor,
        string? status,
        string? from,
        string? subject,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var parsedStatus = ParseInboxStatus(status, out var statusFilter, out var statusError);
        if (!parsedStatus)
        {
            return RequestResult<CursorPaginationResult<EmailInboxMessageDto>>.Failure("emails.inbox.invalid_status", statusError);
        }

        var normalizedPageNumber = pageNumber < 1 ? 1 : pageNumber;
        var normalizedPageSize = pageSize is < 1 or > MaxPageSize ? 20 : pageSize;
        var normalizedOrderBy = string.IsNullOrWhiteSpace(orderBy) ? "ReceivedAtUtc" : orderBy;
        var normalizedOrderDir = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        if (CursorToken.TryParse(cursor, out var token) &&
            normalizedOrderBy.Equals("ReceivedAtUtc", StringComparison.OrdinalIgnoreCase))
        {
            if (token.Parts.Count >= 2 &&
                DateTime.TryParse(token.Parts[0], out var cursorTime) &&
                Guid.TryParse(token.Parts[1], out var cursorId))
            {
                var items = await _inboxRepository.ListByReceivedAtCursorAsync(
                    normalizedPageSize,
                    normalizedOrderDir,
                    cursorTime,
                    cursorId,
                    statusFilter,
                    includeInactive,
                    cancellationToken);

                var dtos = items.Select(MapInbox).ToList();
                var nextCursor = dtos.Count == 0
                    ? null
                    : CursorToken.Build(dtos[^1].ReceivedAtUtc.ToString("O"), dtos[^1].Id);

                return RequestResult<CursorPaginationResult<EmailInboxMessageDto>>.Success(
                    new CursorPaginationResult<EmailInboxMessageDto>(dtos, nextCursor, normalizedPageSize, normalizedOrderBy, normalizedOrderDir));
            }
        }

        var list = await _inboxRepository.ListAsync(
            normalizedPageNumber,
            normalizedPageSize,
            normalizedOrderBy,
            normalizedOrderDir,
            cursor,
            statusFilter,
            from,
            subject,
            includeInactive,
            cancellationToken);

        var fallbackDtos = list.Select(MapInbox).ToList();
        return RequestResult<CursorPaginationResult<EmailInboxMessageDto>>.Success(
            new CursorPaginationResult<EmailInboxMessageDto>(fallbackDtos, null, normalizedPageSize, normalizedOrderBy, normalizedOrderDir));
    }

    public async Task<RequestResult<int>> SyncInboxAsync(int maxMessages, CancellationToken cancellationToken)
    {
        var normalizedMax = maxMessages is < 1 or > 200 ? 50 : maxMessages;
        var items = await _inboxReader.ReadAsync(new EmailInboxReadRequest(normalizedMax), cancellationToken);

        var added = 0;
        foreach (var item in items)
        {
            if (await _inboxRepository.ExistsByProviderMessageIdAsync(item.ProviderMessageId, cancellationToken))
            {
                continue;
            }

            var inbox = new EmailInboxMessage
            {
                ProviderMessageId = item.ProviderMessageId,
                From = item.From,
                To = item.To,
                Subject = item.Subject,
                ReceivedAtUtc = item.ReceivedAtUtc,
                BodyPreview = item.BodyPreview,
                Status = EmailInboxStatus.New
            };

            await _inboxRepository.AddAsync(inbox, cancellationToken);
            added++;
        }

        if (added > 0)
        {
            await _inboxRepository.SaveChangesAsync(cancellationToken);
        }

        return RequestResult<int>.Success(added);
    }

    private static EmailMessageDto MapMessage(EmailMessage message)
        => new(message.Id, message.From, message.To, message.Subject, message.Status, message.AttemptCount, message.CreatedAtUtc, message.SentAtUtc, message.LastError);

    private static EmailInboxMessageDto MapInbox(EmailInboxMessage message)
        => new(message.Id, message.ProviderMessageId, message.From, message.To, message.Subject, message.ReceivedAtUtc, message.Status, message.ProcessingAttempts, message.CreatedAtUtc, message.LastError);

    private static bool ParseStatus(string? status, out EmailMessageStatus? parsed, out string error)
    {
        parsed = null;
        error = string.Empty;
        if (string.IsNullOrWhiteSpace(status))
        {
            return true;
        }

        if (Enum.TryParse<EmailMessageStatus>(status, true, out var value))
        {
            parsed = value;
            return true;
        }

        error = "Invalid email status filter.";
        return false;
    }

    private static bool ParseInboxStatus(string? status, out EmailInboxStatus? parsed, out string error)
    {
        parsed = null;
        error = string.Empty;
        if (string.IsNullOrWhiteSpace(status))
        {
            return true;
        }

        if (Enum.TryParse<EmailInboxStatus>(status, true, out var value))
        {
            parsed = value;
            return true;
        }

        error = "Invalid inbox status filter.";
        return false;
    }
}

