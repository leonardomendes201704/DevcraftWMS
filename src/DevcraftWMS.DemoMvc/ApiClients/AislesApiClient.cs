using DevcraftWMS.DemoMvc.ViewModels.Aisles;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class AislesApiClient : ApiClientBase
{
    public AislesApiClient(
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

    public Task<ApiResult<PagedResultDto<AisleListItemViewModel>>> ListAsync(AisleQuery query, CancellationToken cancellationToken)
    {
        var basePath = $"/api/sections/{query.SectionId}/aisles";
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
        return GetAsync<PagedResultDto<AisleListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<AisleDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<AisleDetailViewModel>($"/api/aisles/{id}", cancellationToken);

    public Task<ApiResult<AisleDetailViewModel>> CreateAsync(Guid sectionId, AisleFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<AisleDetailViewModel>($"/api/sections/{sectionId}/aisles", new AisleCreateRequest(
            sectionId,
            payload.Code,
            payload.Name), cancellationToken);

    public Task<ApiResult<AisleDetailViewModel>> UpdateAsync(Guid id, AisleFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<AisleDetailViewModel>($"/api/aisles/{id}", new AisleUpdateRequest(
            payload.SectionId,
            payload.Code,
            payload.Name), cancellationToken);

    public Task<ApiResult<AisleDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<AisleDetailViewModel>($"/api/aisles/{id}", cancellationToken);
}

public sealed record AisleCreateRequest(Guid SectionId, string Code, string Name);

public sealed record AisleUpdateRequest(Guid SectionId, string Code, string Name);
