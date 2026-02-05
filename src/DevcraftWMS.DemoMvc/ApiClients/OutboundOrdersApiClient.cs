using DevcraftWMS.DemoMvc.ViewModels.OutboundOrders;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class OutboundOrdersApiClient : ApiClientBase
{
    public OutboundOrdersApiClient(
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

    public Task<ApiResult<PagedResultDto<OutboundOrderListItemViewModel>>> ListAsync(OutboundOrderQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/outbound-orders";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["orderNumber"] = query.OrderNumber,
            ["status"] = query.Status?.ToString(),
            ["priority"] = query.Priority?.ToString(),
            ["createdFromUtc"] = query.CreatedFromUtc.HasValue ? query.CreatedFromUtc.Value.ToString("yyyy-MM-dd") : null,
            ["createdToUtc"] = query.CreatedToUtc.HasValue ? query.CreatedToUtc.Value.ToString("yyyy-MM-dd") : null,
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<OutboundOrderListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<OutboundOrderDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<OutboundOrderDetailViewModel>($"/api/outbound-orders/{id}", cancellationToken);

    public Task<ApiResult<OutboundOrderDetailViewModel>> ReleaseAsync(Guid id, object payload, CancellationToken cancellationToken)
        => PostAsync<OutboundOrderDetailViewModel>($"/api/outbound-orders/{id}/release", payload, cancellationToken);
}

