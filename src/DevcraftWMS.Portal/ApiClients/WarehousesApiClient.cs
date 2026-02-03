using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.ApiClients;

public sealed class WarehousesApiClient : ApiClientBase
{
    public WarehousesApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<PagedResultDto<WarehouseOptionDto>>> ListAsync(int pageSize, CancellationToken cancellationToken)
    {
        var basePath = "/api/warehouses";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = "1",
            ["pageSize"] = pageSize.ToString(),
            ["orderBy"] = "Name",
            ["orderDir"] = "asc",
            ["includeInactive"] = "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<WarehouseOptionDto>>(url, cancellationToken);
    }
}
