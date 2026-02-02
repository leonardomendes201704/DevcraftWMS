using DevcraftWMS.DemoMvc.ViewModels.Uoms;
using DevcraftWMS.DemoMvc.Enums;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class UomsApiClient : ApiClientBase
{
    public UomsApiClient(
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

    public Task<ApiResult<PagedResultDto<UomListItemViewModel>>> ListAsync(UomQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/uoms";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["code"] = query.Code,
            ["name"] = query.Name,
            ["type"] = query.Type?.ToString(),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<UomListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<UomDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<UomDetailViewModel>($"/api/uoms/{id}", cancellationToken);

    public Task<ApiResult<UomDetailViewModel>> CreateAsync(UomFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<UomDetailViewModel>("/api/uoms", new CreateUomRequest(payload.Code, payload.Name, payload.Type), cancellationToken);

    public Task<ApiResult<UomDetailViewModel>> UpdateAsync(Guid id, UomFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<UomDetailViewModel>($"/api/uoms/{id}", new UpdateUomRequest(payload.Code, payload.Name, payload.Type), cancellationToken);

    public Task<ApiResult<UomDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<UomDetailViewModel>($"/api/uoms/{id}", cancellationToken);
}

public sealed record CreateUomRequest(string Code, string Name, UomType Type);

public sealed record UpdateUomRequest(string Code, string Name, UomType Type);
