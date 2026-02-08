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
        var basePath = "/api/locations";
        var queryParams = new Dictionary<string, string?>
        {
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["sectorId"] = query.SectorId?.ToString(),
            ["sectionId"] = query.SectionId?.ToString(),
            ["structureId"] = query.StructureId?.ToString(),
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
            payload.Code,
            payload.Barcode,
            payload.Level,
            payload.Row,
            payload.Column,
            payload.ZoneId,
            payload.MaxWeightKg,
            payload.MaxVolumeM3,
            payload.AllowLotTracking,
            payload.AllowExpiryTracking), cancellationToken);

    public Task<ApiResult<LocationDetailViewModel>> UpdateAsync(Guid id, LocationFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<LocationDetailViewModel>($"/api/locations/{id}", new LocationUpdateRequest(
            payload.StructureId,
            payload.Code,
            payload.Barcode,
            payload.Level,
            payload.Row,
            payload.Column,
            payload.ZoneId,
            payload.MaxWeightKg,
            payload.MaxVolumeM3,
            payload.AllowLotTracking,
            payload.AllowExpiryTracking), cancellationToken);

    public Task<ApiResult<LocationDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<LocationDetailViewModel>($"/api/locations/{id}", cancellationToken);
}

public sealed record LocationCreateRequest(
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column,
    Guid? ZoneId,
    decimal? MaxWeightKg,
    decimal? MaxVolumeM3,
    bool AllowLotTracking,
    bool AllowExpiryTracking);

public sealed record LocationUpdateRequest(
    Guid StructureId,
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column,
    Guid? ZoneId,
    decimal? MaxWeightKg,
    decimal? MaxVolumeM3,
    bool AllowLotTracking,
    bool AllowExpiryTracking);
