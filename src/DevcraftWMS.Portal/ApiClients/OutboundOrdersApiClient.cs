using DevcraftWMS.Portal.ViewModels.OutboundOrders;
using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.ApiClients;

public sealed class OutboundOrdersApiClient : ApiClientBase
{
    public OutboundOrdersApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<PagedResultDto<OutboundOrderListItemDto>>> ListAsync(OutboundOrderListQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/outbound-orders";
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
            ["createdFromUtc"] = query.CreatedFromUtc.HasValue ? query.CreatedFromUtc.Value.ToString("yyyy-MM-dd") : null,
            ["createdToUtc"] = query.CreatedToUtc.HasValue ? query.CreatedToUtc.Value.ToString("yyyy-MM-dd") : null,
            ["isActive"] = query.IsActive?.ToString().ToLowerInvariant(),
            ["includeInactive"] = query.IncludeInactive ? "true" : null
        };

        var url = BuildUrl(basePath, qs);
        return GetAsync<PagedResultDto<OutboundOrderListItemDto>>(url, cancellationToken);
    }

    public Task<ApiResult<OutboundOrderDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<OutboundOrderDetailDto>($"/api/outbound-orders/{id}", cancellationToken);

    public Task<ApiResult<OutboundOrderDetailDto>> CreateAsync(object payload, CancellationToken cancellationToken)
        => PostAsync<OutboundOrderDetailDto>("/api/outbound-orders", payload, cancellationToken);
}

