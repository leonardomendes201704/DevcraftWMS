using System.Diagnostics;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.Application.Abstractions.Auth;
using PerfectApiTemplate.Application.Abstractions.Logging;

namespace PerfectApiTemplate.Api.Services;

public sealed class CorrelationContext : ICorrelationContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;
    private readonly LoggingOptions _options;

    public CorrelationContext(
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService,
        IOptions<LoggingOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
        _options = options.Value;
    }

    public string? CorrelationId => GetItem("X-Correlation-ID");

    public string? RequestId => GetItem(_options.RequestIdHeader);

    public string? TraceId => Activity.Current?.TraceId.ToString();

    public string? SpanId => Activity.Current?.SpanId.ToString();

    public string? TenantId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null)
            {
                return null;
            }

            var tenantHeader = _options.Enrichment.TenantHeader;
            if (!string.IsNullOrWhiteSpace(tenantHeader) &&
                context.Request.Headers.TryGetValue(tenantHeader, out var header))
            {
                return header.ToString();
            }

            var claim = context.User.Claims.FirstOrDefault(x => x.Type == _options.Enrichment.TenantClaim);
            return claim?.Value;
        }
    }

    public Guid? UserId => _currentUserService.UserId;

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
