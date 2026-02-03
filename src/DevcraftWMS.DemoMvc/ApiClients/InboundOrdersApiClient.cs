using DevcraftWMS.DemoMvc.ViewModels.InboundOrders;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class InboundOrdersApiClient : ApiClientBase
{
    public InboundOrdersApiClient(
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

    public Task<ApiResult<PagedResultDto<InboundOrderListItemViewModel>>> ListAsync(InboundOrderQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/inbound-orders";
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
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<InboundOrderListItemViewModel>>(url, cancellationToken);
    }
}
