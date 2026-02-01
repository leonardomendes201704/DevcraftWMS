using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.Application.Abstractions.Notifications;

namespace PerfectApiTemplate.Infrastructure.Messaging;

public sealed class OutboxProcessorOptions
{
    public int BatchSize { get; set; } = 20;
    public int PollingSeconds { get; set; } = 5;
}

public sealed class OutboxProcessor : BackgroundService
{
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OutboxProcessorOptions _options;

    public OutboxProcessor(
        ILogger<OutboxProcessor> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<OutboxProcessorOptions> options)
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
                var repository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var publisher = scope.ServiceProvider.GetRequiredService<INotificationPublisher>();

                var messages = await repository.GetUnprocessedAsync(_options.BatchSize, stoppingToken);
                if (messages.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(_options.PollingSeconds), stoppingToken);
                    continue;
                }

                foreach (var message in messages)
                {
                    try
                    {
                        await publisher.PublishAsync(message.Type, message.Payload, stoppingToken);
                        await repository.MarkProcessedAsync(message, stoppingToken);
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
                        _logger.LogError(ex, "Failed to publish outbox message {MessageId}", message.Id);
                        await repository.MarkFailedAsync(message, ex.Message, stoppingToken);
                    }
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
}

