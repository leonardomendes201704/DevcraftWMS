using DevcraftWMS.DemoMvc.ViewModels.Locations;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class LocationsApiClient : ApiClientBase
{
    public LocationsApiClient(
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

    public Task<ApiResult<PagedResultDto<LocationListItemViewModel>>> ListAsync(LocationQuery query, CancellationToken cancellationToken)
    {
        var basePath = $"/api/structures/{query.StructureId}/locations";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["zoneId"] = query.ZoneId?.ToString(),
            ["code"] = query.Code,
            ["barcode"] = query.Barcode,
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<LocationListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<LocationDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<LocationDetailViewModel>($"/api/locations/{id}", cancellationToken);

    public Task<ApiResult<LocationDetailViewModel>> CreateAsync(Guid structureId, LocationFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<LocationDetailViewModel>($"/api/structures/{structureId}/locations", new LocationCreateRequest(
            structureId,
            payload.Code,
            payload.Barcode,
            payload.Level,
            payload.Row,
            payload.Column,
            payload.ZoneId), cancellationToken);

    public Task<ApiResult<LocationDetailViewModel>> UpdateAsync(Guid id, LocationFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<LocationDetailViewModel>($"/api/locations/{id}", new LocationUpdateRequest(
            payload.StructureId,
            payload.Code,
            payload.Barcode,
            payload.Level,
            payload.Row,
            payload.Column,
            payload.ZoneId), cancellationToken);

    public Task<ApiResult<LocationDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<LocationDetailViewModel>($"/api/locations/{id}", cancellationToken);
}

public sealed record LocationCreateRequest(Guid StructureId, string Code, string Barcode, int Level, int Row, int Column, Guid? ZoneId);

public sealed record LocationUpdateRequest(Guid StructureId, string Code, string Barcode, int Level, int Row, int Column, Guid? ZoneId);
