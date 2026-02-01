namespace DevcraftWMS.DemoMvc.Infrastructure.ExternalServices;

public sealed class ExternalServicesOptions
{
    public IbgeOptions Ibge { get; set; } = new();
    public ViaCepOptions ViaCep { get; set; } = new();
}

public sealed class IbgeOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string StatesPath { get; set; } = string.Empty;
    public string CitiesByStatePath { get; set; } = string.Empty;
}

public sealed class ViaCepOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string LookupPath { get; set; } = string.Empty;
}
