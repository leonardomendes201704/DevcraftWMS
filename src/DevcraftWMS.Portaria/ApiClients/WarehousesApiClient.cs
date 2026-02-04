namespace DevcraftWMS.Portaria.ApiClients;

public sealed class WarehousesApiClient : ApiClientBase
{
    public WarehousesApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<PagedResultDto<WarehouseListItemDto>>> ListAsync(WarehouseListQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/warehouses";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["search"] = query.Search,
            ["code"] = query.Code,
            ["name"] = query.Name,
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<WarehouseListItemDto>>(url, cancellationToken);
    }
}

public sealed record WarehouseListItemDto(Guid Id, string Code, string Name, bool IsActive);

public sealed record WarehouseListQuery(
    int PageNumber = 1,
    int PageSize = 100,
    string OrderBy = "Name",
    string OrderDir = "asc",
    string? Search = null,
    string? Code = null,
    string? Name = null,
    bool IncludeInactive = false);
