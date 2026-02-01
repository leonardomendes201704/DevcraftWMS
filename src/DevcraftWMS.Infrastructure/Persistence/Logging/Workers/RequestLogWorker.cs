using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence.Logging.Entities;
using DevcraftWMS.Infrastructure.Persistence.Logging.Queues;

namespace DevcraftWMS.Infrastructure.Persistence.Logging.Workers;

public sealed class RequestLogWorker : BackgroundService
{
    private readonly LogQueue<RequestLog> _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DevcraftWMS.Application.Abstractions.Notifications.IRealtimeNotifier _notifier;
    private readonly ILogger<RequestLogWorker> _logger;

    public RequestLogWorker(
        LogQueue<RequestLog> queue,
        IServiceScopeFactory scopeFactory,
        DevcraftWMS.Application.Abstractions.Notifications.IRealtimeNotifier notifier,
        ILogger<RequestLogWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _notifier = notifier;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var log in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
                    db.RequestLogs.Add(log);
                    await db.SaveChangesAsync(stoppingToken);
                    await _notifier.PublishAsync(
                        "logs.requests",
                        "requestLog.created",
                        new { log.Id, log.Path, log.StatusCode, log.StartedAtUtc },
                        stoppingToken);
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
                    _logger.LogWarning(ex, "Failed to persist request log");
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

