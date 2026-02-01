using Microsoft.AspNetCore.Html;

namespace PerfectApiTemplate.DemoMvc.ViewModels.Shared;

public sealed class GridBuilder<T>
{
    private readonly List<T> _items = new();
    private readonly List<GridColumn<T>> _columns = new();
    private readonly List<GridRowAction<T>> _actions = new();
    private PaginationViewModel _pagination = new();
    private GridQueryOptions _queryOptions = new();
    private GridFilterViewModel? _filters;
    private string _title = string.Empty;
    private string? _subtitle;
    private string? _primaryActionText;
    private string? _primaryActionUrl;
    private string _emptyMessage = "No records found.";
    private string _tableCssClass = "table align-middle mb-0 grid-table";
    private string _cardCssClass = "card";

    public GridBuilder<T> WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public GridBuilder<T> WithSubtitle(string? subtitle)
    {
        _subtitle = subtitle;
        return this;
    }

    public GridBuilder<T> WithPrimaryAction(string text, string url)
    {
        _primaryActionText = text;
        _primaryActionUrl = url;
        return this;
    }

    public GridBuilder<T> WithItems(IEnumerable<T> items)
    {
        _items.Clear();
        _items.AddRange(items);
        return this;
    }

    public GridBuilder<T> WithPagination(PaginationViewModel pagination)
    {
        _pagination = pagination;
        return this;
    }

    public GridBuilder<T> WithQueryOptions(GridQueryOptions options)
    {
        _queryOptions = options;
        return this;
    }

    public GridBuilder<T> WithFilters(GridFilterViewModel? filters)
    {
        _filters = filters;
        return this;
    }

    public GridBuilder<T> WithEmptyMessage(string message)
    {
        _emptyMessage = message;
        return this;
    }

    public GridBuilder<T> WithTableCssClass(string cssClass)
    {
        _tableCssClass = cssClass;
        return this;
    }

    public GridBuilder<T> WithCardCssClass(string cssClass)
    {
        _cardCssClass = cssClass;
        return this;
    }

    public GridBuilder<T> AddColumn(string header, Func<T, IHtmlContent> cell, string? sortKey = null, string? headerCssClass = null, string? cellCssClass = null)
    {
        _columns.Add(new GridColumn<T>
        {
            Header = header,
            Cell = cell,
            SortKey = sortKey,
            HeaderCssClass = headerCssClass,
            CellCssClass = cellCssClass
        });
        return this;
    }

    public GridBuilder<T> AddAction(string text, Func<T, string?> url, string cssClass = "btn btn-sm btn-outline-secondary", string? iconClass = null)
    {
        _actions.Add(new GridRowAction<T>
        {
            Text = text,
            Url = url,
            CssClass = cssClass,
            IconClass = iconClass
        });
        return this;
    }

    public GridViewModel<object> Build()
    {
        var objectColumns = _columns
            .Select(column => new GridColumn<object>
            {
                Header = column.Header,
                SortKey = column.SortKey,
                HeaderCssClass = column.HeaderCssClass,
                CellCssClass = column.CellCssClass,
                Cell = item => column.Cell((T)item)
            })
            .ToList();

        var objectActions = _actions
            .Select(action => new GridRowAction<object>
            {
                Text = action.Text,
                CssClass = action.CssClass,
                IconClass = action.IconClass,
                Url = item => action.Url((T)item)
            })
            .ToList();

        return new GridViewModel<object>
        {
            Title = _title,
            Subtitle = _subtitle,
            PrimaryActionText = _primaryActionText,
            PrimaryActionUrl = _primaryActionUrl,
            Items = _items.Cast<object>().ToList(),
            Columns = objectColumns,
            RowActions = objectActions,
            Pagination = _pagination,
            QueryOptions = _queryOptions,
            Filters = _filters,
            EmptyMessage = _emptyMessage,
            TableCssClass = _tableCssClass,
            CardCssClass = _cardCssClass
        };
    }
}
