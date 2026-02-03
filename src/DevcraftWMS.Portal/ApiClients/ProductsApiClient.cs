using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.ApiClients;

public sealed class ProductsApiClient : ApiClientBase
{
    public ProductsApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<PagedResultDto<ProductOptionDto>>> ListAsync(int pageSize, CancellationToken cancellationToken)
    {
        var basePath = "/api/products";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = "1",
            ["pageSize"] = pageSize.ToString(),
            ["orderBy"] = "Name",
            ["orderDir"] = "asc",
            ["includeInactive"] = "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<ProductOptionDto>>(url, cancellationToken);
    }
}
