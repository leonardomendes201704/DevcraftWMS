namespace DevcraftWMS.Portal.ApiClients;

public sealed class CustomersApiClient : ApiClientBase
{
    public CustomersApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<PagedResultDto<CustomerDto>>> ListAsync(CustomerListQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/customers/paged";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["search"] = query.Search,
            ["email"] = query.Email,
            ["name"] = query.Name,
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<CustomerDto>>(url, cancellationToken);
    }
}

public sealed record CustomerDto(Guid Id, string Name, string Email, DateOnly DateOfBirth, DateTime CreatedAtUtc);

public sealed record CustomerListQuery(
    int PageNumber = 1,
    int PageSize = 100,
    string OrderBy = "Name",
    string OrderDir = "asc",
    string? Search = null,
    string? Email = null,
    string? Name = null,
    bool IncludeInactive = false);

public sealed record PagedResultDto<T>(IReadOnlyList<T> Items, int TotalCount, int PageNumber, int PageSize, string OrderBy, string OrderDir);
