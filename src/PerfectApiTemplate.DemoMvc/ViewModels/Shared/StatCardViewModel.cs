namespace PerfectApiTemplate.DemoMvc.ViewModels.Shared;

public sealed class StatCardViewModel
{
    public string Title { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string? Subtitle { get; init; }
    public string? BadgeText { get; init; }
    public string BadgeCssClass { get; init; } = "text-bg-secondary";
    public string? ActionText { get; init; }
    public string? ActionUrl { get; init; }
}
