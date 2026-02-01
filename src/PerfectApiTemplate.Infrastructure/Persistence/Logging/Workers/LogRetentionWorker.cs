using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using PerfectApiTemplate.Application.Abstractions.Logging;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Workers;

public sealed class LogRetentionWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LogRetentionWorker> _logger;
    private readonly LoggingOptions _options;

    public LogRetentionWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<LogRetentionWorker> logger,
        IOptions<LoggingOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Retention.Enabled)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunCleanupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Log retention cleanup failed");
            }

            await Task.Delay(TimeSpan.FromMinutes(_options.Retention.RunIntervalMinutes), stoppingToken);
        }
    }

    private async Task RunCleanupAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LogsDbContext>();

        var now = DateTime.UtcNow;
        if (_options.RetentionDays.Requests > 0)
        {
            var cutoff = now.AddDays(-_options.RetentionDays.Requests);
            await db.RequestLogs.Where(x => x.StartedAtUtc < cutoff).ExecuteDeleteAsync(cancellationToken);
        }

        if (_options.RetentionDays.Errors > 0)
        {
            var cutoff = now.AddDays(-_options.RetentionDays.Errors);
            await db.ErrorLogs.Where(x => x.CreatedAtUtc < cutoff).ExecuteDeleteAsync(cancellationToken);
        }

        if (_options.RetentionDays.Transactions > 0)
        {
            var cutoff = now.AddDays(-_options.RetentionDays.Transactions);
            await db.TransactionLogs.Where(x => x.CreatedAtUtc < cutoff).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
