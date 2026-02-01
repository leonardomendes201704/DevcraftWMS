using PerfectApiTemplate.DemoMvc.ApiClients;
using PerfectApiTemplate.DemoMvc.ViewModels.Shared;

namespace PerfectApiTemplate.DemoMvc.ViewModels.Customers;

public sealed class CustomerListViewModel
{
    public IReadOnlyList<CustomerDto> Items { get; init; } = Array.Empty<CustomerDto>();
    public CustomerListQuery Query { get; init; } = new();
    public PaginationViewModel Pagination { get; init; } = new();
}

public sealed class CustomerListQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = "CreatedAtUtc";
    public string OrderDir { get; set; } = "desc";
    public string? Search { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public bool IncludeInactive { get; set; }
}
