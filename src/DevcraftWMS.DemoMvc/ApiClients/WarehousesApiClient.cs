using DevcraftWMS.DemoMvc.ViewModels.Warehouses;
using DevcraftWMS.DemoMvc.Infrastructure;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class WarehousesApiClient : ApiClientBase
{
    public WarehousesApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ApiUrlProvider urlProvider,
        DevcraftWMS.DemoMvc.Infrastructure.Telemetry.IClientCorrelationContext correlationContext,
        DevcraftWMS.DemoMvc.Infrastructure.Telemetry.IClientTelemetryDispatcher telemetryDispatcher,
        Microsoft.Extensions.Options.IOptionsMonitor<DevcraftWMS.DemoMvc.Infrastructure.Telemetry.ClientTelemetryOptions> telemetryOptions,
        IWebHostEnvironment environment)
        : base(httpClient, httpContextAccessor, urlProvider, correlationContext, telemetryDispatcher, telemetryOptions, environment)
    {
    }

    public Task<ApiResult<PagedResultDto<WarehouseListItemViewModel>>> ListAsync(WarehouseQuery query, CancellationToken cancellationToken)
    {
        var basePath = "api/warehouses";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["search"] = query.Search,
            ["code"] = query.Code,
            ["name"] = query.Name,
            ["warehouseType"] = query.WarehouseType?.ToString(),
            ["city"] = query.City,
            ["state"] = query.State,
            ["country"] = query.Country,
            ["externalId"] = query.ExternalId,
            ["erpCode"] = query.ErpCode,
            ["costCenterCode"] = query.CostCenterCode,
            ["isPrimary"] = query.IsPrimary is null ? null : (query.IsPrimary.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var path = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<WarehouseListItemViewModel>>(path, cancellationToken);
    }

    public Task<ApiResult<WarehouseDetailsViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<WarehouseDetailsViewModel>($"api/warehouses/{id}", cancellationToken);

    public Task<ApiResult<WarehouseDetailsViewModel>> CreateAsync(WarehouseFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<WarehouseDetailsViewModel>("api/warehouses", payload, cancellationToken);

    public Task<ApiResult<WarehouseDetailsViewModel>> UpdateAsync(Guid id, WarehouseFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<WarehouseDetailsViewModel>($"api/warehouses/{id}", payload, cancellationToken);

    public Task<ApiResult<WarehouseDetailsViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<WarehouseDetailsViewModel>($"api/warehouses/{id}", cancellationToken);
}
