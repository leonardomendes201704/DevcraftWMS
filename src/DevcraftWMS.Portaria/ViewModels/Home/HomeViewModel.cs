namespace DevcraftWMS.Portaria.ViewModels.Home;

public sealed class HomeViewModel
{
    public bool IsApiHealthy { get; init; }
    public string StatusMessage { get; init; } = string.Empty;
}
