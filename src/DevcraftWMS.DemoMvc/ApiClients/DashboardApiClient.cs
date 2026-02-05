using DevcraftWMS.DemoMvc.ViewModels.Dashboard;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class DashboardApiClient : ApiClientBase
{
    public DashboardApiClient(
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

    public Task<ApiResult<ExpiringLotsKpiDto>> GetExpiringLotsAsync(int? days, CancellationToken cancellationToken)
    {
        var basePath = "/api/dashboard/expiring-lots";
        var queryParams = new Dictionary<string, string?>
        {
            ["days"] = days?.ToString()
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<ExpiringLotsKpiDto>(url, cancellationToken);
    }

    public Task<ApiResult<InboundKpiDto>> GetInboundKpisAsync(int? days, CancellationToken cancellationToken)
    {
        var basePath = "/api/dashboard/inbound-kpis";
        var queryParams = new Dictionary<string, string?>
        {
            ["days"] = days?.ToString()
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<InboundKpiDto>(url, cancellationToken);
    }

    public Task<ApiResult<OutboundKpiDto>> GetOutboundKpisAsync(int? days, CancellationToken cancellationToken)
    {
        var basePath = "/api/dashboard/outbound-kpis";
        var queryParams = new Dictionary<string, string?>
        {
            ["days"] = days?.ToString()
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<OutboundKpiDto>(url, cancellationToken);
    }
}
