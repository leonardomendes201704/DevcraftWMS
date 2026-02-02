namespace DevcraftWMS.Portal.ApiClients;

public sealed class AuthApiClient : ApiClientBase
{
    public AuthApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<AuthResponseDto>> LoginAsync(string email, string password, CancellationToken cancellationToken)
        => PostAsync<AuthResponseDto>("/api/auth/login", new { email, password }, cancellationToken);
}

public sealed record AuthResponseDto(Guid UserId, string Email, string Token);
