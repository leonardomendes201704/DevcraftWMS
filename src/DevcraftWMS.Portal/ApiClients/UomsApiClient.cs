using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.ApiClients;

public sealed class UomsApiClient : ApiClientBase
{
    public UomsApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<PagedResultDto<UomOptionDto>>> ListAsync(int pageSize, CancellationToken cancellationToken)
    {
        var basePath = "/api/uoms";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = "1",
            ["pageSize"] = pageSize.ToString(),
            ["orderBy"] = "Code",
            ["orderDir"] = "asc",
            ["includeInactive"] = "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<UomOptionDto>>(url, cancellationToken);
    }
}
