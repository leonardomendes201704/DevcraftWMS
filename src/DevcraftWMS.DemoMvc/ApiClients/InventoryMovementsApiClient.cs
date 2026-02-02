using DevcraftWMS.DemoMvc.ViewModels.InventoryMovements;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class InventoryMovementsApiClient : ApiClientBase
{
    public InventoryMovementsApiClient(
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

    public Task<ApiResult<PagedResultDto<InventoryMovementListItemViewModel>>> ListAsync(InventoryMovementQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/inventory/movements";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["productId"] = query.ProductId?.ToString(),
            ["fromLocationId"] = query.FromLocationId?.ToString(),
            ["toLocationId"] = query.ToLocationId?.ToString(),
            ["lotId"] = query.LotId?.ToString(),
            ["status"] = query.Status?.ToString(),
            ["performedFromUtc"] = query.PerformedFromUtc?.ToString("yyyy-MM-dd"),
            ["performedToUtc"] = query.PerformedToUtc?.ToString("yyyy-MM-dd"),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<InventoryMovementListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<InventoryMovementDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<InventoryMovementDetailViewModel>($"/api/inventory/movements/{id}", cancellationToken);

    public Task<ApiResult<InventoryMovementDetailViewModel>> CreateAsync(InventoryMovementFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<InventoryMovementDetailViewModel>("/api/inventory/movements", new CreateInventoryMovementRequest(
            payload.FromLocationId,
            payload.ToLocationId,
            payload.ProductId,
            payload.LotId,
            payload.Quantity,
            payload.Reason,
            payload.Reference,
            payload.PerformedAtUtc), cancellationToken);
}

public sealed record CreateInventoryMovementRequest(
    Guid FromLocationId,
    Guid ToLocationId,
    Guid ProductId,
    Guid? LotId,
    decimal Quantity,
    string? Reason,
    string? Reference,
    DateTime? PerformedAtUtc);
