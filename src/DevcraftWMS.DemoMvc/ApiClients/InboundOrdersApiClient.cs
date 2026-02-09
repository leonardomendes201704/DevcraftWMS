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
            ["createdFromUtc"] = query.CreatedFromUtc.HasValue ? query.CreatedFromUtc.Value.ToString("yyyy-MM-dd") : null,
            ["createdToUtc"] = query.CreatedToUtc.HasValue ? query.CreatedToUtc.Value.ToString("yyyy-MM-dd") : null,
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<InboundOrderListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<InboundOrderReceiptReportViewModel>> GetReportAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<InboundOrderReceiptReportViewModel>($"/api/inbound-orders/{id}/report", cancellationToken);

    public Task<ApiFileResult> ExportReportAsync(Guid id, CancellationToken cancellationToken)
        => GetFileAsync($"/api/inbound-orders/{id}/report/export", cancellationToken);

    public Task<ApiResult<object>> ApproveEmergencyAsync(Guid id, string? notes, CancellationToken cancellationToken)
        => PostAsync<object>($"/api/inbound-orders/{id}/approve-emergency", new { notes }, cancellationToken);

    public Task<ApiResult<InboundOrderListItemViewModel>> ConvertFromAsnAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => PostAsync<InboundOrderListItemViewModel>("/api/inbound-orders/from-asn", new { asnId, notes }, cancellationToken);
}
