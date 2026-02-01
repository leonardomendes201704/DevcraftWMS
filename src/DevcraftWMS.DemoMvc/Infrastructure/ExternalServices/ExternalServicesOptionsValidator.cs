using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.DemoMvc.Infrastructure.ExternalServices;

public static class ExternalServicesOptionsValidator
{
    public static bool TryValidate(ExternalServicesOptions options, [NotNullWhen(false)] out string? error)
    {
        if (options is null)
        {
            error = "ExternalServices configuration is missing.";
            return false;
        }

        if (!ValidateService("Ibge", options.Ibge.BaseUrl, options.Ibge.StatesPath, options.Ibge.CitiesByStatePath, out error))
        {
            return false;
        }

        if (!ValidateService("ViaCep", options.ViaCep.BaseUrl, options.ViaCep.LookupPath, null, out error))
        {
            return false;
        }

        error = null;
        return true;
    }

    private static bool ValidateService(
        string name,
        string baseUrl,
        string path,
        string? optionalPath,
        [NotNullWhen(false)] out string? error)
    {
        if (!IsAbsoluteUrl(baseUrl))
        {
            error = $"ExternalServices:{name}:BaseUrl must be an absolute URL.";
            return false;
        }

        if (!IsPath(path))
        {
            error = $"ExternalServices:{name}:Path values must start with '/' and cannot be empty.";
            return false;
        }

        if (optionalPath is not null && !IsPath(optionalPath))
        {
            error = $"ExternalServices:{name}:Path values must start with '/' and cannot be empty.";
            return false;
        }

        error = null;
        return true;
    }

    private static bool IsAbsoluteUrl(string? value)
        => !string.IsNullOrWhiteSpace(value)
           && Uri.TryCreate(value, UriKind.Absolute, out _);

    private static bool IsPath(string? value)
        => !string.IsNullOrWhiteSpace(value) && value.StartsWith("/", StringComparison.Ordinal);
}

public sealed class ExternalServicesOptionsValidation : IValidateOptions<ExternalServicesOptions>
{
    public ValidateOptionsResult Validate(string? name, ExternalServicesOptions options)
    {
        return ExternalServicesOptionsValidator.TryValidate(options, out var error)
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(error ?? "ExternalServices configuration is invalid.");
    }
}
