using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.Application.Abstractions.Email;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Infrastructure.Email;

public sealed class EmailOutboxProcessor : BackgroundService
{
    private readonly ILogger<EmailOutboxProcessor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly EmailProcessingOptions _options;

    public EmailOutboxProcessor(
        ILogger<EmailOutboxProcessor> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<EmailProcessingOptions> options)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IEmailMessageRepository>();
                var sender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                var messages = await repository.GetPendingAsync(_options.BatchSize, _options.MaxAttempts, stoppingToken);
                if (messages.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(_options.OutboxPollingSeconds), stoppingToken);
                    continue;
                }

                foreach (var message in messages)
                {
                    await ProcessMessageAsync(repository, sender, message, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    private async Task ProcessMessageAsync(
        IEmailMessageRepository repository,
        IEmailSender sender,
        EmailMessage message,
        CancellationToken cancellationToken)
    {
        try
        {
            message.Status = EmailMessageStatus.Sending;
            await repository.SaveChangesAsync(cancellationToken);

            var sendResult = await sender.SendAsync(
                new EmailSendRequest(message.From, message.To, message.Subject, message.Body, message.IsHtml),
                cancellationToken);

            message.AttemptCount += 1;
            message.LastAttemptAtUtc = DateTime.UtcNow;

            if (sendResult.Success)
            {
                message.Status = EmailMessageStatus.Sent;
                message.SentAtUtc = DateTime.UtcNow;
                message.ProviderMessageId = sendResult.ProviderMessageId;
                message.LastError = null;

                await repository.AddAttemptAsync(new EmailDeliveryAttempt
                {
                    EmailMessageId = message.Id,
                    AttemptedAtUtc = DateTime.UtcNow,
                    Succeeded = true
                }, cancellationToken);
            }
            else
            {
                message.Status = EmailMessageStatus.Failed;
                message.LastError = sendResult.Error;
                await repository.AddAttemptAsync(new EmailDeliveryAttempt
                {
                    EmailMessageId = message.Id,
                    AttemptedAtUtc = DateTime.UtcNow,
                    Succeeded = false,
                    Error = sendResult.Error
                }, cancellationToken);
            }

            await repository.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process email message {EmailMessageId}", message.Id);
            message.AttemptCount += 1;
            message.LastAttemptAtUtc = DateTime.UtcNow;
            message.Status = EmailMessageStatus.Failed;
            message.LastError = ex.Message;

            await repository.AddAttemptAsync(new EmailDeliveryAttempt
            {
                EmailMessageId = message.Id,
                AttemptedAtUtc = DateTime.UtcNow,
                Succeeded = false,
                Error = ex.Message
            }, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
        }
    }
}
