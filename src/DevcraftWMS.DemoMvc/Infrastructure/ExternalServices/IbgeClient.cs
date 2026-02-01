using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.DemoMvc.Infrastructure.ExternalServices;

public interface IIbgeClient
{
    Task<IReadOnlyList<IbgeState>> GetStatesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<IbgeCity>> GetCitiesAsync(string uf, CancellationToken cancellationToken);
}

public sealed class IbgeClient : IIbgeClient
{
    private readonly HttpClient _httpClient;
    private readonly ExternalServicesOptions _options;

    public IbgeClient(HttpClient httpClient, IOptions<ExternalServicesOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<IbgeState>> GetStatesAsync(CancellationToken cancellationToken)
    {
        var data = await _httpClient.GetFromJsonAsync<List<IbgeStateDto>>(_options.Ibge.StatesPath, cancellationToken)
                   ?? new List<IbgeStateDto>();

        return data
            .Where(state => !string.IsNullOrWhiteSpace(state.Abbreviation))
            .OrderBy(state => state.Name)
            .Select(state => new IbgeState(state.Abbreviation!, state.Name ?? state.Abbreviation!))
            .ToList();
    }

    public async Task<IReadOnlyList<IbgeCity>> GetCitiesAsync(string uf, CancellationToken cancellationToken)
    {
        var normalized = uf.Trim().ToUpperInvariant();
        var path = _options.Ibge.CitiesByStatePath.Replace("{uf}", Uri.EscapeDataString(normalized), StringComparison.Ordinal);

        var data = await _httpClient.GetFromJsonAsync<List<IbgeCityDto>>(path, cancellationToken)
                   ?? new List<IbgeCityDto>();

        return data
            .Where(city => !string.IsNullOrWhiteSpace(city.Name))
            .OrderBy(city => city.Name)
            .Select(city => new IbgeCity(city.Name!))
            .ToList();
    }

    private sealed record IbgeStateDto(string? Sigla, string? Nome)
    {
        public string? Abbreviation => Sigla;
        public string? Name => Nome;
    }

    private sealed record IbgeCityDto(string? Nome)
    {
        public string? Name => Nome;
    }
}

public sealed record IbgeState(string Code, string Name);
public sealed record IbgeCity(string Name);
