using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.DemoMvc.Infrastructure.ExternalServices;

public interface IViaCepClient
{
    Task<ViaCepResult?> LookupAsync(string cep, CancellationToken cancellationToken);
}

public sealed class ViaCepClient : IViaCepClient
{
    private readonly HttpClient _httpClient;
    private readonly ExternalServicesOptions _options;

    public ViaCepClient(HttpClient httpClient, IOptions<ExternalServicesOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<ViaCepResult?> LookupAsync(string cep, CancellationToken cancellationToken)
    {
        var normalized = new string((cep ?? string.Empty).Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return null;
        }

        var path = _options.ViaCep.LookupPath.Replace("{cep}", Uri.EscapeDataString(normalized), StringComparison.Ordinal);
        var response = await _httpClient.GetFromJsonAsync<ViaCepDto>(path, cancellationToken);
        if (response is null || response.HasError)
        {
            return null;
        }

        return new ViaCepResult(
            normalized,
            response.Logradouro ?? string.Empty,
            response.Bairro ?? string.Empty,
            response.Localidade ?? string.Empty,
            response.Uf ?? string.Empty);
    }

    private sealed record ViaCepDto(
        string? Cep,
        string? Logradouro,
        string? Bairro,
        string? Localidade,
        string? Uf,
        bool? Erro)
    {
        public bool HasError => Erro.GetValueOrDefault();
    }
}

public sealed record ViaCepResult(
    string PostalCode,
    string AddressLine1,
    string District,
    string City,
    string State);
