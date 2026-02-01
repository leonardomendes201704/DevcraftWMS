using System.ComponentModel.DataAnnotations;

namespace PerfectApiTemplate.DemoMvc.ViewModels.Settings;

public sealed class SettingsViewModel
{
    [Required]
    [Url]
    public string ApiBaseUrl { get; set; } = string.Empty;

    [Required]
    public string TelemetryEndpointPath { get; set; } = "/api/telemetry/client-events";

    [Range(1, 1000)]
    public int TelemetryBatchSize { get; set; } = 20;

    [Range(100, 60000)]
    public int TelemetryFlushIntervalMs { get; set; } = 2000;

    [Range(10, 10000)]
    public int TelemetryQueueCapacity { get; set; } = 500;

    [Range(100, 60000)]
    public int TelemetrySlowCallThresholdMs { get; set; } = 1500;

    [Range(0, 10)]
    public int TelemetryRetryCount { get; set; } = 2;

    [Required]
    public string TelemetryInternalKeyHeader { get; set; } = "X-Internal-Telemetry-Key";

    public string TelemetryInternalKey { get; set; } = string.Empty;

    [Range(1, 50)]
    public int TelemetryCircuitFailureThreshold { get; set; } = 5;

    [Range(5, 600)]
    public int TelemetryCircuitBreakDurationSeconds { get; set; } = 30;
}
