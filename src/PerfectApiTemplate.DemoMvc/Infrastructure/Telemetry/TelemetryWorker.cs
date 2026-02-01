using System.Threading.Channels;
using Microsoft.Extensions.Options;

namespace PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry;

public sealed class TelemetryWorker : BackgroundService
{
    private readonly ClientTelemetryQueue _queue;
    private readonly TelemetryApiClient _client;
    private readonly IOptionsMonitor<ClientTelemetryOptions> _options;
    private readonly ILogger<TelemetryWorker> _logger;
    private int _failureCount;
    private DateTimeOffset? _circuitOpenUntil;

    public TelemetryWorker(
        ClientTelemetryQueue queue,
        TelemetryApiClient client,
        IOptionsMonitor<ClientTelemetryOptions> options,
        ILogger<TelemetryWorker> logger)
    {
        _queue = queue;
        _client = client;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var buffer = new List<ClientTelemetryEvent>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_circuitOpenUntil.HasValue && _circuitOpenUntil > DateTimeOffset.UtcNow)
                {
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                while (buffer.Count < _options.CurrentValue.BatchSize && _queue.Reader.TryRead(out var item))
                {
                    buffer.Add(item);
                }

                if (buffer.Count == 0)
                {
                    var delay = Task.Delay(_options.CurrentValue.FlushIntervalMs, stoppingToken);
                    var readTask = _queue.Reader.WaitToReadAsync(stoppingToken).AsTask();
                    await Task.WhenAny(delay, readTask);
                    continue;
                }

                await FlushAsync(buffer, stoppingToken);
                buffer.Clear();
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Telemetry worker failure");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private async Task FlushAsync(List<ClientTelemetryEvent> events, CancellationToken cancellationToken)
    {
        foreach (var telemetryEvent in events)
        {
            var success = await SendWithRetryAsync(telemetryEvent, cancellationToken);
            if (!success)
            {
                RegisterFailure();
            }
        }
    }

    private async Task<bool> SendWithRetryAsync(ClientTelemetryEvent telemetryEvent, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt <= _options.CurrentValue.RetryCount; attempt++)
        {
            var success = await _client.SendAsync(telemetryEvent, cancellationToken);
            if (success)
            {
                _failureCount = 0;
                return true;
            }

            if (attempt < _options.CurrentValue.RetryCount)
            {
                await Task.Delay(200 * (attempt + 1), cancellationToken);
            }
        }

        return false;
    }

    private void RegisterFailure()
    {
        _failureCount++;
        if (_failureCount >= _options.CurrentValue.CircuitBreaker.FailureThreshold)
        {
            _circuitOpenUntil = DateTimeOffset.UtcNow.AddSeconds(_options.CurrentValue.CircuitBreaker.BreakDurationSeconds);
            _logger.LogWarning("Telemetry circuit breaker opened for {Seconds}s", _options.CurrentValue.CircuitBreaker.BreakDurationSeconds);
        }
    }
}
