using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.InventoryBalances;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class InventoryBalancesApiClient : ApiClientBase
{
    public InventoryBalancesApiClient(
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

    public Task<ApiResult<PagedResultDto<InventoryBalanceListItemViewModel>>> ListAsync(InventoryBalanceQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/inventory/balances";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["locationId"] = query.LocationId?.ToString(),
            ["productId"] = query.ProductId?.ToString(),
            ["lotId"] = query.LotId?.ToString(),
            ["status"] = query.Status?.ToString(),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<InventoryBalanceListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<PagedResultDto<InventoryBalanceListItemViewModel>>> ListByLocationAsync(Guid locationId, InventoryBalanceQuery query, CancellationToken cancellationToken)
    {
        var basePath = $"/api/locations/{locationId}/inventory";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["productId"] = query.ProductId?.ToString(),
            ["lotId"] = query.LotId?.ToString(),
            ["status"] = query.Status?.ToString(),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<InventoryBalanceListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<InventoryBalanceDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<InventoryBalanceDetailViewModel>($"/api/inventory/balances/{id}", cancellationToken);

    public Task<ApiResult<InventoryBalanceDetailViewModel>> CreateAsync(Guid locationId, InventoryBalanceFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<InventoryBalanceDetailViewModel>($"/api/locations/{locationId}/inventory", new CreateInventoryBalanceRequest(
            payload.ProductId,
            payload.LotId,
            payload.QuantityOnHand,
            payload.QuantityReserved,
            payload.Status), cancellationToken);

    public Task<ApiResult<InventoryBalanceDetailViewModel>> UpdateAsync(Guid id, InventoryBalanceFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<InventoryBalanceDetailViewModel>($"/api/inventory/balances/{id}", new UpdateInventoryBalanceRequest(
            payload.LocationId,
            payload.ProductId,
            payload.LotId,
            payload.QuantityOnHand,
            payload.QuantityReserved,
            payload.Status), cancellationToken);

    public Task<ApiResult<InventoryBalanceDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<InventoryBalanceDetailViewModel>($"/api/inventory/balances/{id}", cancellationToken);
}

public sealed record CreateInventoryBalanceRequest(
    Guid ProductId,
    Guid? LotId,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    InventoryBalanceStatus Status);

public sealed record UpdateInventoryBalanceRequest(
    Guid LocationId,
    Guid ProductId,
    Guid? LotId,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    InventoryBalanceStatus Status);
