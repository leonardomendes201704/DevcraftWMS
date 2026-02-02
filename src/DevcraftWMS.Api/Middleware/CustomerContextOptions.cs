namespace DevcraftWMS.Api.Middleware;

public sealed class CustomerContextOptions
{
    public string HeaderName { get; init; } = "X-Customer-Id";
    public string[] ExcludedPaths { get; init; } = new[]
    {
        "/swagger",
        "/health",
        "/api/auth",
        "/api/customers",
        "/api/telemetry",
        "/api/settings"
    };
}
