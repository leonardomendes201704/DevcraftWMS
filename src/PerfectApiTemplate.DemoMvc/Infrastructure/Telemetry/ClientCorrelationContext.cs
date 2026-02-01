namespace PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry;

public interface IClientCorrelationContext
{
    string CorrelationId { get; }
    string? ClientRequestId { get; }
}

public sealed class ClientCorrelationContext : IClientCorrelationContext
{
    public const string CorrelationHeader = "X-Correlation-ID";
    public const string ClientRequestHeader = "X-Client-Request-Id";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientCorrelationContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string CorrelationId
        => GetItem(CorrelationHeader) ?? Guid.NewGuid().ToString("N");

    public string? ClientRequestId => GetItem(ClientRequestHeader);

    private string? GetItem(string key)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            return null;
        }

        return context.Items.TryGetValue(key, out var value) ? value as string : null;
    }
}
