using DevcraftWMS.Portal.ViewModels.InboundOrders;
using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.ApiClients;

public sealed class InboundOrdersApiClient : ApiClientBase
{
    public InboundOrdersApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<PagedResultDto<InboundOrderListItemDto>>> ListAsync(InboundOrderListQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/inbound-orders";
        var qs = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["orderNumber"] = query.OrderNumber,
            ["status"] = query.Status?.ToString(),
            ["priority"] = query.Priority?.ToString(),
            ["isActive"] = query.IsActive?.ToString().ToLowerInvariant(),
            ["includeInactive"] = query.IncludeInactive ? "true" : null
        };

        var url = BuildUrl(basePath, qs);
        return GetAsync<PagedResultDto<InboundOrderListItemDto>>(url, cancellationToken);
    }

    public Task<ApiResult<InboundOrderDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<InboundOrderDetailDto>($"/api/inbound-orders/{id}", cancellationToken);

    public Task<ApiResult<InboundOrderDetailDto>> ConvertFromAsnAsync(ConvertInboundOrderRequest request, CancellationToken cancellationToken)
        => PostAsync<InboundOrderDetailDto>("/api/inbound-orders/from-asn", request, cancellationToken);

    public Task<ApiResult<InboundOrderDetailDto>> UpdateParametersAsync(Guid id, UpdateInboundOrderParametersRequest request, CancellationToken cancellationToken)
        => PutAsync<InboundOrderDetailDto>($"/api/inbound-orders/{id}/parameters", request, cancellationToken);

    public Task<ApiResult<InboundOrderDetailDto>> CancelAsync(Guid id, CancelInboundOrderRequest request, CancellationToken cancellationToken)
        => PostAsync<InboundOrderDetailDto>($"/api/inbound-orders/{id}/cancel", request, cancellationToken);

    public Task<ApiResult<InboundOrderDetailDto>> CompleteAsync(Guid id, CompleteInboundOrderRequest request, CancellationToken cancellationToken)
        => PostAsync<InboundOrderDetailDto>($"/api/inbound-orders/{id}/complete", request, cancellationToken);

    public Task<ApiResult<InboundOrderReceiptReportDto>> GetReportAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<InboundOrderReceiptReportDto>($"/api/inbound-orders/{id}/report", cancellationToken);

    public Task<ApiFileResult> ExportReportAsync(Guid id, CancellationToken cancellationToken)
        => GetFileAsync($"/api/inbound-orders/{id}/report/export", cancellationToken);

    public Task<ApiResult<IReadOnlyList<InboundOrderNotificationDto>>> ListNotificationsAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<IReadOnlyList<InboundOrderNotificationDto>>($"/api/inbound-orders/{id}/notifications", cancellationToken);

    public Task<ApiResult<InboundOrderNotificationDto>> ResendNotificationAsync(Guid inboundOrderId, Guid notificationId, CancellationToken cancellationToken)
        => PostAsync<InboundOrderNotificationDto>($"/api/inbound-orders/{inboundOrderId}/notifications/{notificationId}/resend", new { }, cancellationToken);
}
