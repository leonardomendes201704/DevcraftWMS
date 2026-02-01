using Microsoft.AspNetCore.Html;

namespace PerfectApiTemplate.DemoMvc.ViewModels.Shared;

public sealed class GridViewModel<T>
{
    public string Title { get; init; } = string.Empty;
    public string? Subtitle { get; init; }
    public string? PrimaryActionText { get; init; }
    public string? PrimaryActionUrl { get; init; }
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public IReadOnlyList<GridColumn<T>> Columns { get; init; } = Array.Empty<GridColumn<T>>();
    public IReadOnlyList<GridRowAction<T>> RowActions { get; init; } = Array.Empty<GridRowAction<T>>();
    public PaginationViewModel Pagination { get; init; } = new();
    public GridQueryOptions QueryOptions { get; init; } = new();
    public GridFilterViewModel? Filters { get; init; }
    public string EmptyMessage { get; init; } = "No records found.";
    public string TableCssClass { get; init; } = "table align-middle mb-0 grid-table";
    public string CardCssClass { get; init; } = "card";
}

public sealed class GridQueryOptions
{
    public string Action { get; init; } = string.Empty;
    public string Controller { get; init; } = string.Empty;
    public Dictionary<string, string?> Query { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public string? OrderBy { get; init; }
    public string? OrderDir { get; init; }
    public int PageSize { get; init; }
}

public sealed class GridColumn<T>
{
    public string Header { get; init; } = string.Empty;
    public Func<T, IHtmlContent> Cell { get; init; } = _ => HtmlString.Empty;
    public string? SortKey { get; init; }
    public string? HeaderCssClass { get; init; }
    public string? CellCssClass { get; init; }
    public bool IsSortable => !string.IsNullOrWhiteSpace(SortKey);
}

public sealed class GridRowAction<T>
{
    public string Text { get; init; } = string.Empty;
    public string CssClass { get; init; } = "btn btn-sm btn-outline-secondary";
    public string? IconClass { get; init; }
    public Func<T, string?> Url { get; init; } = _ => null;
}

public sealed class GridFilterViewModel
{
    public string Action { get; init; } = string.Empty;
    public string Controller { get; init; } = string.Empty;
    public IReadOnlyList<GridFilterField> Fields { get; init; } = Array.Empty<GridFilterField>();
    public string SubmitText { get; init; } = "Filter";
    public string? ResetUrl { get; init; }
    public bool ShowReset { get; init; }
}

public sealed class GridFilterField
{
    public string Label { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Value { get; init; }
    public GridFilterType Type { get; init; } = GridFilterType.Text;
    public IReadOnlyList<GridSelectOption> Options { get; init; } = Array.Empty<GridSelectOption>();
    public string? Placeholder { get; init; }
    public bool IsChecked { get; init; }
    public string? CssClass { get; init; }
    public bool IsHidden { get; init; }
}

public enum GridFilterType
{
    Text,
    Select,
    Checkbox,
    Date
}

public sealed class GridSelectOption
{
    public string Value { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
    public bool Selected { get; init; }
}
