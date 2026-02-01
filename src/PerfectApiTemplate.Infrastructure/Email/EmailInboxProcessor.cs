using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.Application.Abstractions.Email;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Infrastructure.Email;

public sealed class EmailInboxProcessor : BackgroundService
{
    private readonly ILogger<EmailInboxProcessor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly EmailProcessingOptions _options;

    public EmailInboxProcessor(
        ILogger<EmailInboxProcessor> logger,
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
                var repository = scope.ServiceProvider.GetRequiredService<IEmailInboxRepository>();
                var reader = scope.ServiceProvider.GetRequiredService<IEmailInboxReader>();

                try
                {
                    var items = await reader.ReadAsync(new EmailInboxReadRequest(_options.BatchSize), stoppingToken);
                    var added = 0;

                    foreach (var item in items)
                    {
                        if (await repository.ExistsByProviderMessageIdAsync(item.ProviderMessageId, stoppingToken))
                        {
                            continue;
                        }

                        await repository.AddAsync(new EmailInboxMessage
                        {
                            ProviderMessageId = item.ProviderMessageId,
                            From = item.From,
                            To = item.To,
                            Subject = item.Subject,
                            ReceivedAtUtc = item.ReceivedAtUtc,
                            BodyPreview = item.BodyPreview,
                            Status = EmailInboxStatus.New
                        }, stoppingToken);

                        added++;
                    }

                    if (added > 0)
                    {
                        await repository.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process email inbox batch");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.InboxPollingSeconds), stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }
}
