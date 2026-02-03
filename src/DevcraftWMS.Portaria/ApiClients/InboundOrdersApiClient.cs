using DevcraftWMS.Portaria.ViewModels.InboundOrders;

namespace DevcraftWMS.Portaria.ApiClients;

public sealed class InboundOrdersApiClient : ApiClientBase
{
    public InboundOrdersApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<PagedResultDto<InboundOrderListItemDto>>> ListAsync(InboundOrderListQuery query, CancellationToken cancellationToken)
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
            ["isActive"] = query.IsActive?.ToString()?.ToLowerInvariant(),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<InboundOrderListItemDto>>(url, cancellationToken);
    }
}
