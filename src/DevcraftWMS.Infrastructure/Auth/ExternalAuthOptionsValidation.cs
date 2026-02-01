using Microsoft.Extensions.Options;

namespace DevcraftWMS.Infrastructure.Auth;

public sealed class ExternalAuthOptionsValidation : IValidateOptions<ExternalAuthOptions>
{
    public ValidateOptionsResult Validate(string? name, ExternalAuthOptions options)
    {
        if (options.Providers.Count == 0)
        {
            return ValidateOptionsResult.Fail("ExternalAuth providers are not configured.");
        }

        foreach (var (providerName, provider) in options.Providers)
        {
            if (string.IsNullOrWhiteSpace(provider.UserInfoUrl))
            {
                return ValidateOptionsResult.Fail($"ExternalAuth provider '{providerName}' is missing UserInfoUrl.");
            }

            if (!Uri.TryCreate(provider.UserInfoUrl, UriKind.Absolute, out _))
            {
                return ValidateOptionsResult.Fail($"ExternalAuth provider '{providerName}' has invalid UserInfoUrl.");
            }
        }

        return ValidateOptionsResult.Success;
    }
}
