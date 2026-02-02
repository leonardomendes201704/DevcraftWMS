using Microsoft.Extensions.Options;

namespace DevcraftWMS.Portal.Infrastructure;

public sealed class ApiUrlProvider
{
    private readonly IOptionsMonitor<PortalOptions> _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiUrlProvider(IOptionsMonitor<PortalOptions> options, IHttpContextAccessor httpContextAccessor)
    {
        _options = options;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetBaseUrl()
    {
        var context = _httpContextAccessor.HttpContext;
        var overrideUrl = context?.Session.GetStringValue(SessionKeys.ApiBaseUrl);

        if (!string.IsNullOrWhiteSpace(overrideUrl))
        {
            return overrideUrl;
        }

        return _options.CurrentValue.ApiBaseUrl;
    }
}
