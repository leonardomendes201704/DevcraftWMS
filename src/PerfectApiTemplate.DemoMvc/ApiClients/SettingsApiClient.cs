namespace PerfectApiTemplate.DemoMvc.ApiClients;

public sealed class SettingsApiClient : ApiClientBase
{
    public SettingsApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider,
        Infrastructure.Telemetry.IClientCorrelationContext correlationContext,
        Infrastructure.Telemetry.IClientTelemetryDispatcher telemetryDispatcher,
        Microsoft.Extensions.Options.IOptionsMonitor<Infrastructure.Telemetry.ClientTelemetryOptions> telemetryOptions,
        IWebHostEnvironment environment)
        : base(httpClient, httpContextAccessor, urlProvider, correlationContext, telemetryDispatcher, telemetryOptions, environment)
    {
    }

    public Task<ApiResult<ApiSettingsDto>> GetSettingsAsync(CancellationToken cancellationToken)
        => GetAsync<ApiSettingsDto>("/api/settings", cancellationToken);
}

public sealed record ApiSettingsDto(IReadOnlyList<ApiSettingsSectionDto> Sections, DateTimeOffset RetrievedAtUtc);

public sealed record ApiSettingsSectionDto(string Name, IReadOnlyList<ApiSettingItemDto> Items);

public sealed record ApiSettingItemDto(string Key, string Value, string Description);
