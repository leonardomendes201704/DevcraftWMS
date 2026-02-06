using DevcraftWMS.DemoMvc.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevcraftWMS.DemoMvc.ViewModels.Returns;

public sealed class ReturnListPageViewModel
{
    public ReturnListQueryViewModel Query { get; set; } = new(null, null, null, null, false, 1, 20, "CreatedAtUtc", "desc");
    public IReadOnlyList<ReturnOrderListItemViewModel> Items { get; set; } = Array.Empty<ReturnOrderListItemViewModel>();
    public PaginationViewModel? Pagination { get; set; }
    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class ReturnDetailsPageViewModel
{
    public ReturnOrderViewModel Order { get; set; } = null!;
}
