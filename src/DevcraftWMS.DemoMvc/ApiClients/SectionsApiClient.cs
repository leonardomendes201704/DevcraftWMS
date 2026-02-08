using DevcraftWMS.DemoMvc.ViewModels.Sections;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class SectionsApiClient : ApiClientBase
{
    public SectionsApiClient(
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

    public Task<ApiResult<PagedResultDto<SectionListItemViewModel>>> ListAsync(SectionQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/sections";
        var queryParams = new Dictionary<string, string?>
        {
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["sectorId"] = query.SectorId?.ToString(),
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["code"] = query.Code,
            ["name"] = query.Name,
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<SectionListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<SectionDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<SectionDetailViewModel>($"/api/sections/{id}", cancellationToken);

    public Task<ApiResult<SectionDetailViewModel>> CreateAsync(Guid sectorId, SectionFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<SectionDetailViewModel>($"/api/sectors/{sectorId}/sections", new SectionCreateRequest(
            sectorId,
            payload.Code,
            payload.Name,
            payload.Description), cancellationToken);

    public Task<ApiResult<SectionDetailViewModel>> UpdateAsync(Guid id, SectionFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<SectionDetailViewModel>($"/api/sections/{id}", new SectionUpdateRequest(
            payload.SectorId,
            payload.Code,
            payload.Name,
            payload.Description), cancellationToken);

    public Task<ApiResult<SectionDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<SectionDetailViewModel>($"/api/sections/{id}", cancellationToken);
}

public sealed record SectionCreateRequest(Guid SectorId, string Code, string Name, string? Description);

public sealed record SectionUpdateRequest(Guid SectorId, string Code, string Name, string? Description);
