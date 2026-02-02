namespace DevcraftWMS.Portal.ViewModels.Shared;

public sealed class BreadcrumbItem
{
    public string Title { get; init; } = string.Empty;
    public string Url { get; init; } = "#";
    public bool IsActive { get; init; }
}
