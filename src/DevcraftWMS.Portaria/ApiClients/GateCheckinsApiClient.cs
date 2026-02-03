using DevcraftWMS.Portaria.ViewModels.GateCheckins;

namespace DevcraftWMS.Portaria.ApiClients;

public sealed class GateCheckinsApiClient : ApiClientBase
{
    public GateCheckinsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<PagedResultDto<GateCheckinListItemDto>>> ListAsync(GateCheckinListQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/gate/checkins";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["inboundOrderId"] = query.InboundOrderId?.ToString(),
            ["documentNumber"] = query.DocumentNumber,
            ["vehiclePlate"] = query.VehiclePlate,
            ["driverName"] = query.DriverName,
            ["carrierName"] = query.CarrierName,
            ["status"] = query.Status.HasValue ? ((int)query.Status.Value).ToString() : null,
            ["arrivalFromUtc"] = query.ArrivalFromUtc?.ToString("O"),
            ["arrivalToUtc"] = query.ArrivalToUtc?.ToString("O"),
            ["isActive"] = query.IsActive?.ToString()?.ToLowerInvariant(),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<GateCheckinListItemDto>>(url, cancellationToken);
    }

    public Task<ApiResult<GateCheckinDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<GateCheckinDetailDto>($"/api/gate/checkins/{id}", cancellationToken);

    public Task<ApiResult<GateCheckinDetailDto>> CreateAsync(GateCheckinCreateRequest request, CancellationToken cancellationToken)
        => PostAsync<GateCheckinDetailDto>("/api/gate/checkins", request, cancellationToken);

    public Task<ApiResult<GateCheckinDetailDto>> UpdateAsync(Guid id, GateCheckinUpdateRequest request, CancellationToken cancellationToken)
        => PutAsync<GateCheckinDetailDto>($"/api/gate/checkins/{id}", request, cancellationToken);

    public Task<ApiResult<GateCheckinDetailDto>> AssignDockAsync(Guid id, string dockCode, CancellationToken cancellationToken)
        => PostAsync<GateCheckinDetailDto>($"/api/gate/checkins/{id}/assign-dock", new { dockCode }, cancellationToken);

    public Task<ApiResult<GateCheckinDetailDto>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<GateCheckinDetailDto>($"/api/gate/checkins/{id}", cancellationToken);
}

public sealed record GateCheckinCreateRequest(
    Guid? InboundOrderId,
    string? DocumentNumber,
    string VehiclePlate,
    string DriverName,
    string? CarrierName,
    DateTime? ArrivalAtUtc,
    string? Notes);

public sealed record GateCheckinUpdateRequest(
    Guid? InboundOrderId,
    string? DocumentNumber,
    string VehiclePlate,
    string DriverName,
    string? CarrierName,
    DateTime ArrivalAtUtc,
    int Status,
    string? Notes);
