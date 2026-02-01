namespace PerfectApiTemplate.DemoMvc.ApiClients;

public sealed class HealthApiClient : ApiClientBase
{
    public HealthApiClient(
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

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken)
    {
        var result = await GetAsync<string>("/health", cancellationToken);
        return result.IsSuccess;
    }
}
