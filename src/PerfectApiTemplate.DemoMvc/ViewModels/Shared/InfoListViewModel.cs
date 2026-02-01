namespace PerfectApiTemplate.DemoMvc.ViewModels.Shared;

public sealed class InfoListItem
{
    public string Label { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string? BadgeText { get; init; }
    public string BadgeCssClass { get; init; } = "text-bg-light border";
}

public sealed class InfoListViewModel
{
    public string Title { get; init; } = string.Empty;
    public IReadOnlyList<InfoListItem> Items { get; init; } = Array.Empty<InfoListItem>();
}
