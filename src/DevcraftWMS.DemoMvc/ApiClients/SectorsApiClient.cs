using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Sectors;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class SectorsApiClient : ApiClientBase
{
    public SectorsApiClient(
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

    public Task<ApiResult<PagedResultDto<SectorListItemViewModel>>> ListAsync(SectorQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/sectors";
        var queryParams = new Dictionary<string, string?>
        {
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["code"] = query.Code,
            ["name"] = query.Name,
            ["sectorType"] = query.SectorType?.ToString(),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<SectorListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<SectorDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<SectorDetailViewModel>($"/api/sectors/{id}", cancellationToken);

    public Task<ApiResult<SectorDetailViewModel>> CreateAsync(Guid warehouseId, SectorFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<SectorDetailViewModel>($"/api/warehouses/{warehouseId}/sectors", new SectorCreateRequest(
            warehouseId,
            payload.Code,
            payload.Name,
            payload.Description,
            payload.SectorType), cancellationToken);

    public Task<ApiResult<SectorDetailViewModel>> UpdateAsync(Guid id, SectorFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<SectorDetailViewModel>($"/api/sectors/{id}", new SectorUpdateRequest(
            payload.WarehouseId,
            payload.Code,
            payload.Name,
            payload.Description,
            payload.SectorType), cancellationToken);

    public Task<ApiResult<SectorDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<SectorDetailViewModel>($"/api/sectors/{id}", cancellationToken);
}

public sealed record SectorCreateRequest(Guid WarehouseId, string Code, string Name, string? Description, SectorType SectorType);

public sealed record SectorUpdateRequest(Guid WarehouseId, string Code, string Name, string? Description, SectorType SectorType);
