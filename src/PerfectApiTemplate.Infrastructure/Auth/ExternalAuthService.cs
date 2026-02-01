using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.Application.Abstractions.Auth;

namespace PerfectApiTemplate.Infrastructure.Auth;

public sealed class ExternalAuthOptions
{
    public Dictionary<string, ExternalProviderOptions> Providers { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ExternalProviderOptions
{
    public string UserInfoUrl { get; set; } = string.Empty;
}

public sealed class ExternalAuthService : IExternalAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ExternalAuthOptions _options;

    public ExternalAuthService(HttpClient httpClient, IOptions<ExternalAuthOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<ExternalUserInfo> GetUserInfoAsync(string provider, string accessToken, CancellationToken cancellationToken = default)
    {
        if (!_options.Providers.TryGetValue(provider, out var providerOptions) || string.IsNullOrWhiteSpace(providerOptions.UserInfoUrl))
        {
            throw new InvalidOperationException($"External provider '{provider}' is not configured.");
        }

        if (provider.Equals("facebook", StringComparison.OrdinalIgnoreCase))
        {
            return await GetFacebookUserInfoAsync(providerOptions.UserInfoUrl, provider, accessToken, cancellationToken);
        }

        return await GetBearerUserInfoAsync(providerOptions.UserInfoUrl, provider, accessToken, cancellationToken);
    }

    private async Task<ExternalUserInfo> GetBearerUserInfoAsync(string url, string provider, string accessToken, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(payload);
        var root = doc.RootElement;

        var providerUserId = GetFirstString(root, "sub", "id")
            ?? throw new InvalidOperationException("Provider user id not found.");

        var email = GetFirstString(root, "email", "mail", "userPrincipalName");
        var name = GetFirstString(root, "name", "displayName");

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Email not provided by external provider.");
        }

        return new ExternalUserInfo(provider, providerUserId, email, name ?? email);
    }

    private async Task<ExternalUserInfo> GetFacebookUserInfoAsync(string url, string provider, string accessToken, CancellationToken cancellationToken)
    {
        var fullUrl = url.Contains("?")
            ? $"{url}&access_token={accessToken}"
            : $"{url}?access_token={accessToken}";

        using var response = await _httpClient.GetAsync(fullUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(payload);
        var root = doc.RootElement;

        var providerUserId = GetFirstString(root, "id") ?? throw new InvalidOperationException("Provider user id not found.");
        var email = GetFirstString(root, "email");
        var name = GetFirstString(root, "name");

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Email not provided by external provider.");
        }

        return new ExternalUserInfo(provider, providerUserId, email, name ?? email);
    }

    private static string? GetFirstString(JsonElement root, params string[] names)
    {
        foreach (var name in names)
        {
            if (root.TryGetProperty(name, out var element))
            {
                var value = element.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
        }

        return null;
    }
}
