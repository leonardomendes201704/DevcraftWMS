namespace PerfectApiTemplate.Application.Abstractions.Auth;

public sealed record ExternalUserInfo(string Provider, string ProviderUserId, string Email, string FullName);

public interface IExternalAuthService
{
    Task<ExternalUserInfo> GetUserInfoAsync(string provider, string accessToken, CancellationToken cancellationToken = default);
}
