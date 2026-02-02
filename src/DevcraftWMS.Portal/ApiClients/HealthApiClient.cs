namespace DevcraftWMS.Portal.ApiClients;

public sealed class HealthApiClient : ApiClientBase
{
    public HealthApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<string>> GetHealthAsync(CancellationToken cancellationToken)
        => GetAsync<string>("/health", cancellationToken);
}
