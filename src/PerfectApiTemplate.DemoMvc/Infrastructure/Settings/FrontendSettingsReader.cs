using Microsoft.Extensions.Configuration;
using PerfectApiTemplate.DemoMvc.ViewModels.Settings;

namespace PerfectApiTemplate.DemoMvc.Infrastructure.Settings;

public sealed class FrontendSettingsReader
{
    private readonly IConfiguration _configuration;

    public FrontendSettingsReader(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IReadOnlyList<SettingsSectionViewModel> BuildSections()
    {
        return new List<SettingsSectionViewModel>
        {
            new()
            {
                Name = "Logging",
                Items = new List<SettingsItemViewModel>
                {
                    Item("Logging:LogLevel:Default", "Default log level for DemoMvc."),
                    Item("Logging:LogLevel:Microsoft.AspNetCore", "Log level for ASP.NET Core logs.")
                }
            },
            new()
            {
                Name = "DemoMvc",
                Items = new List<SettingsItemViewModel>
                {
                    Item("ApiBaseUrl", "Base URL used by DemoMvc to call the API.")
                }
            },
            new()
            {
                Name = "Telemetry",
                Items = new List<SettingsItemViewModel>
                {
                    Item("Telemetry:Enabled", "Enable or disable client telemetry sending."),
                    Item("Telemetry:EndpointPath", "API path for sending client telemetry events."),
                    Item("Telemetry:BatchSize", "Number of telemetry events sent per batch."),
                    Item("Telemetry:FlushIntervalMs", "Flush interval in milliseconds for telemetry batches."),
                    Item("Telemetry:QueueCapacity", "In-memory telemetry queue capacity."),
                    Item("Telemetry:SlowCallThresholdMs", "Slow API call threshold for telemetry."),
                    Item("Telemetry:RetryCount", "Number of retry attempts for telemetry sending."),
                    Item("Telemetry:InternalKeyHeader", "Header name used to send the telemetry internal key."),
                    Item("Telemetry:InternalKey", "Internal key value sent with telemetry requests."),
                    Item("Telemetry:CircuitBreaker:FailureThreshold", "Failures before opening circuit breaker."),
                    Item("Telemetry:CircuitBreaker:BreakDurationSeconds", "Circuit breaker open duration (seconds).")
                }
            },
            new()
            {
                Name = "Demo",
                Items = new List<SettingsItemViewModel>
                {
                    Item("Demo:AdminEmail", "Default admin email shown in Demo UI.")
                }
            },
            new()
            {
                Name = "AllowedHosts",
                Items = new List<SettingsItemViewModel>
                {
                    Item("AllowedHosts", "Allowed hosts for the DemoMvc app.")
                }
            }
        };
    }

    private SettingsItemViewModel Item(string key, string description)
    {
        var value = _configuration[key] ?? string.Empty;
        return new SettingsItemViewModel
        {
            Key = key,
            Value = value,
            Description = description
        };
    }
}
