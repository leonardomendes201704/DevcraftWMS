namespace PerfectApiTemplate.DemoMvc.ViewModels.Shared;

public sealed class FilterDrawerViewModel
{
    public string Id { get; init; } = "filtersDrawer";
    public string Title { get; init; } = "Filters";
    public GridFilterViewModel Filters { get; init; } = new();
    public string ButtonText { get; init; } = "Filters";
    public string ButtonCssClass { get; init; } = "btn btn-outline-secondary";
}
