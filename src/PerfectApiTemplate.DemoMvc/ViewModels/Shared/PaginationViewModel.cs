namespace PerfectApiTemplate.DemoMvc.ViewModels.Shared;

public sealed class PaginationViewModel
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public string Action { get; init; } = string.Empty;
    public string Controller { get; init; } = string.Empty;
    public Dictionary<string, string?> Query { get; init; } = new();

    public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

