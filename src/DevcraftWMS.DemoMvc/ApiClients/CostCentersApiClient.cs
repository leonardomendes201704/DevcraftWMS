using DevcraftWMS.DemoMvc.ViewModels.CostCenters;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class CostCentersApiClient : ApiClientBase
{
    public CostCentersApiClient(
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

    public Task<ApiResult<PagedResultDto<CostCenterListItemViewModel>>> ListAsync(CostCenterQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/cost-centers";
        var queryParams = new Dictionary<string, string?>
        {
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
        return GetAsync<PagedResultDto<CostCenterListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<CostCenterDetailsViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<CostCenterDetailsViewModel>($"/api/cost-centers/{id}", cancellationToken);

    public Task<ApiResult<CostCenterDetailsViewModel>> CreateAsync(CostCenterFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<CostCenterDetailsViewModel>("/api/cost-centers", new CreateCostCenterRequest(payload.Code, payload.Name, payload.Description), cancellationToken);

    public Task<ApiResult<CostCenterDetailsViewModel>> UpdateAsync(Guid id, CostCenterFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<CostCenterDetailsViewModel>($"/api/cost-centers/{id}", new UpdateCostCenterRequest(payload.Code, payload.Name, payload.Description), cancellationToken);

    public Task<ApiResult<CostCenterDetailsViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<CostCenterDetailsViewModel>($"/api/cost-centers/{id}", cancellationToken);
}

public sealed record CreateCostCenterRequest(string Code, string Name, string? Description);

public sealed record UpdateCostCenterRequest(string Code, string Name, string? Description);
