using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.InboundOrders;

public sealed record InboundOrderQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    Guid? WarehouseId = null,
    string? OrderNumber = null,
    int? Status = null,
    int? Priority = null,
    DateTime? CreatedFromUtc = null,
    DateTime? CreatedToUtc = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record InboundOrderListItemViewModel(
    Guid Id,
    string OrderNumber,
    string AsnNumber,
    string WarehouseName,
    int Status,
    int Priority,
    DateOnly? ExpectedArrivalDate,
    DateTime CreatedAtUtc,
    bool IsEmergency,
    bool IsActive);

public sealed class InboundOrderListPageViewModel
{
    public IReadOnlyList<InboundOrderListItemViewModel> Items { get; init; } = Array.Empty<InboundOrderListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public InboundOrderQuery Query { get; init; } = new();
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
}
