namespace PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry;

public sealed class ClientTelemetryOptions
{
    public bool Enabled { get; init; } = true;
    public string EndpointPath { get; init; } = "/api/telemetry/client-events";
    public int BatchSize { get; init; } = 20;
    public int FlushIntervalMs { get; init; } = 2000;
    public int QueueCapacity { get; init; } = 500;
    public int SlowCallThresholdMs { get; init; } = 1500;
    public int RetryCount { get; init; } = 2;
    public string InternalKeyHeader { get; init; } = "X-Internal-Telemetry-Key";
    public string? InternalKey { get; init; }
    public CircuitBreakerOptions CircuitBreaker { get; init; } = new();
}

public sealed class CircuitBreakerOptions
{
    public int FailureThreshold { get; init; } = 5;
    public int BreakDurationSeconds { get; init; } = 30;
}
