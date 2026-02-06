using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.Infrastructure.Telemetry;
using DevcraftWMS.DemoMvc.ViewModels.InventoryCounts;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class InventoryCountsApiClient : ApiClientBase
{
    public InventoryCountsApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ApiUrlProvider urlProvider,
        IClientCorrelationContext correlationContext,
        IClientTelemetryDispatcher telemetryDispatcher,
        IOptionsMonitor<ClientTelemetryOptions> telemetryOptions,
        IWebHostEnvironment environment)
        : base(httpClient, httpContextAccessor, urlProvider, correlationContext, telemetryDispatcher, telemetryOptions, environment)
    {
    }

    public Task<ApiResult<PagedResultDto<InventoryCountListItemViewModel>>> ListAsync(InventoryCountListQueryViewModel query, CancellationToken cancellationToken)
        => GetAsync<PagedResultDto<InventoryCountListItemViewModel>>($"/api/inventory-counts{query.ToQueryString()}", cancellationToken);

    public Task<ApiResult<InventoryCountViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<InventoryCountViewModel>($"/api/inventory-counts/{id}", cancellationToken);

    public Task<ApiResult<InventoryCountViewModel>> CreateAsync(CreateInventoryCountRequestViewModel request, CancellationToken cancellationToken)
        => PostAsync<InventoryCountViewModel>("/api/inventory-counts", request, cancellationToken);

    public Task<ApiResult<InventoryCountViewModel>> StartAsync(Guid id, CancellationToken cancellationToken)
        => PostAsync<InventoryCountViewModel>($"/api/inventory-counts/{id}/start", null, cancellationToken);

    public Task<ApiResult<InventoryCountViewModel>> CompleteAsync(Guid id, CompleteInventoryCountRequestViewModel request, CancellationToken cancellationToken)
        => PostAsync<InventoryCountViewModel>($"/api/inventory-counts/{id}/complete", request, cancellationToken);
}
