using DevcraftWMS.DemoMvc.ViewModels.Zones;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class ZonesApiClient : ApiClientBase
{
    public ZonesApiClient(
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

    public Task<ApiResult<PagedResultDto<ZoneListItemViewModel>>> ListAsync(ZoneQuery query, CancellationToken cancellationToken)
    {
        var basePath = $"/api/warehouses/{query.WarehouseId}/zones";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["code"] = query.Code,
            ["name"] = query.Name,
            ["zoneType"] = query.ZoneType?.ToString(),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<ZoneListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<ZoneDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<ZoneDetailViewModel>($"/api/zones/{id}", cancellationToken);

    public Task<ApiResult<ZoneDetailViewModel>> CreateAsync(Guid warehouseId, ZoneFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<ZoneDetailViewModel>($"/api/warehouses/{warehouseId}/zones", new ZoneCreateRequest(
            payload.Code,
            payload.Name,
            payload.Description,
            payload.ZoneType), cancellationToken);

    public Task<ApiResult<ZoneDetailViewModel>> UpdateAsync(Guid id, ZoneFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<ZoneDetailViewModel>($"/api/zones/{id}", new ZoneUpdateRequest(
            payload.WarehouseId,
            payload.Code,
            payload.Name,
            payload.Description,
            payload.ZoneType), cancellationToken);

    public Task<ApiResult<ZoneDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<ZoneDetailViewModel>($"/api/zones/{id}", cancellationToken);
}

public sealed record ZoneCreateRequest(string Code, string Name, string? Description, Enums.ZoneType ZoneType);

public sealed record ZoneUpdateRequest(Guid WarehouseId, string Code, string Name, string? Description, Enums.ZoneType ZoneType);
