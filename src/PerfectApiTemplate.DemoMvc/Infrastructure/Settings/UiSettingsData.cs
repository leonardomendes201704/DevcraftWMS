namespace PerfectApiTemplate.DemoMvc.Infrastructure.Settings;

public sealed class UiSettingsData
{
    public string ApiBaseUrl { get; set; } = string.Empty;
    public TelemetrySettingsData Telemetry { get; set; } = new();
}

public sealed class TelemetrySettingsData
{
    public string EndpointPath { get; set; } = "/api/telemetry/client-events";
    public int BatchSize { get; set; } = 20;
    public int FlushIntervalMs { get; set; } = 2000;
    public int QueueCapacity { get; set; } = 500;
    public int SlowCallThresholdMs { get; set; } = 1500;
    public int RetryCount { get; set; } = 2;
    public string InternalKeyHeader { get; set; } = "X-Internal-Telemetry-Key";
    public string InternalKey { get; set; } = string.Empty;
    public CircuitBreakerSettingsData CircuitBreaker { get; set; } = new();
}

public sealed class CircuitBreakerSettingsData
{
    public int FailureThreshold { get; set; } = 5;
    public int BreakDurationSeconds { get; set; } = 30;
}
