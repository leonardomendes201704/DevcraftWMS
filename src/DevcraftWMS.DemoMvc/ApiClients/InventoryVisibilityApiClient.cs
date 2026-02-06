using DevcraftWMS.DemoMvc.ViewModels.InventoryVisibility;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class InventoryVisibilityApiClient : ApiClientBase
{
    public InventoryVisibilityApiClient(
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

    public Task<ApiResult<InventoryVisibilityResultViewModel>> GetAsync(InventoryVisibilityQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/inventory-visibility";
        var queryParams = new Dictionary<string, string?>
        {
            ["customerId"] = query.CustomerId?.ToString(),
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["productId"] = query.ProductId?.ToString(),
            ["sku"] = query.Sku,
            ["lotCode"] = query.LotCode,
            ["expirationFrom"] = query.ExpirationFrom?.ToString("yyyy-MM-dd"),
            ["expirationTo"] = query.ExpirationTo?.ToString("yyyy-MM-dd"),
            ["status"] = query.Status?.ToString(),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false",
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<InventoryVisibilityResultViewModel>(url, cancellationToken);
    }

    public Task<ApiResult<IReadOnlyList<InventoryVisibilityTraceViewModel>>> GetTimelineAsync(
        Guid productId,
        Guid customerId,
        Guid warehouseId,
        string? lotCode,
        Guid? locationId,
        CancellationToken cancellationToken)
    {
        var url = $"/api/inventory-visibility/{productId}/timeline?customerId={customerId}&warehouseId={warehouseId}";

        if (!string.IsNullOrWhiteSpace(lotCode))
        {
            url += $"&lotCode={Uri.EscapeDataString(lotCode)}";
        }

        if (locationId.HasValue)
        {
            url += $"&locationId={locationId}";
        }

        return GetAsync<IReadOnlyList<InventoryVisibilityTraceViewModel>>(url, cancellationToken);
    }
}
