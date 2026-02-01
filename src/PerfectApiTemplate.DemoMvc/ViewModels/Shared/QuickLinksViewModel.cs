namespace PerfectApiTemplate.DemoMvc.ViewModels.Shared;

public sealed class QuickLinkItem
{
    public string Text { get; init; } = string.Empty;
    public string Url { get; init; } = "#";
    public string? IconClass { get; init; }
}

public sealed class QuickLinksViewModel
{
    public string Title { get; init; } = "Quick Links";
    public IReadOnlyList<QuickLinkItem> Links { get; init; } = Array.Empty<QuickLinkItem>();
}
