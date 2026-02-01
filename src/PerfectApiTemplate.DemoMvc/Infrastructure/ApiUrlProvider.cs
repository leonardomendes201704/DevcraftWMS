using Microsoft.Extensions.Options;

namespace PerfectApiTemplate.DemoMvc.Infrastructure;

public sealed class ApiUrlProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Microsoft.Extensions.Options.IOptionsMonitor<DemoOptions> _options;

    public ApiUrlProvider(IHttpContextAccessor httpContextAccessor, Microsoft.Extensions.Options.IOptionsMonitor<DemoOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options;
    }

    public string GetBaseUrl()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is not null)
        {
            var overrideUrl = context.Session.GetStringValue(SessionKeys.ApiBaseUrl);
            if (!string.IsNullOrWhiteSpace(overrideUrl))
            {
                return overrideUrl;
            }
        }

        return _options.CurrentValue.ApiBaseUrl;
    }
}
