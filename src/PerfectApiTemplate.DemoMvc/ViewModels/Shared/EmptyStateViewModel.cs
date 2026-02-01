namespace PerfectApiTemplate.DemoMvc.ViewModels.Shared;

public sealed class EmptyStateViewModel
{
    public string Message { get; init; } = "No data available.";
    public string? ActionText { get; init; }
    public string? ActionUrl { get; init; }
    public string IconClass { get; init; } = "bi bi-inbox";
}
