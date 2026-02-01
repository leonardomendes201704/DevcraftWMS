namespace PerfectApiTemplate.DemoMvc.ApiClients;

public sealed class AuthApiClient : ApiClientBase
{
    public AuthApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider,
        Infrastructure.Telemetry.IClientCorrelationContext correlationContext,
        Infrastructure.Telemetry.IClientTelemetryDispatcher telemetryDispatcher,
        Microsoft.Extensions.Options.IOptionsMonitor<Infrastructure.Telemetry.ClientTelemetryOptions> telemetryOptions,
        IWebHostEnvironment environment)
        : base(httpClient, httpContextAccessor, urlProvider, correlationContext, telemetryDispatcher, telemetryOptions, environment)
    {
    }

    public Task<ApiResult<AuthResponseDto>> LoginAsync(string email, string password, CancellationToken cancellationToken)
        => PostAsync<AuthResponseDto>("/api/auth/login", new { email, password }, cancellationToken);
}

public sealed record AuthResponseDto(Guid UserId, string Email, string Token);
