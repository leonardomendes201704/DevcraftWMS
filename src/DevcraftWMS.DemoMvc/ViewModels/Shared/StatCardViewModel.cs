namespace DevcraftWMS.DemoMvc.ViewModels.Shared;

public sealed class StatCardViewModel
{
    public string Title { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string? Subtitle { get; init; }
    public string? BadgeText { get; init; }
    public string BadgeCssClass { get; init; } = "text-bg-secondary";
    public string? ActionText { get; init; }
    public string? ActionUrl { get; init; }
    public string? IconClass { get; init; }
    public string IconCssClass { get; init; } = "text-bg-light";
}

