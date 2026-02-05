using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.OutboundOrders;

public sealed record OutboundOrderQuery(
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

public sealed record OutboundOrderListItemViewModel(
    Guid Id,
    string OrderNumber,
    string WarehouseName,
    int Status,
    int Priority,
    DateOnly? ExpectedShipDate,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record OutboundOrderItemViewModel(
    Guid Id,
    Guid ProductId,
    Guid UomId,
    string ProductCode,
    string ProductName,
    string UomCode,
    decimal Quantity,
    string? LotCode,
    DateOnly? ExpirationDate);

public sealed record OutboundOrderDetailViewModel(
    Guid Id,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    string? CustomerReference,
    string? CarrierName,
    DateOnly? ExpectedShipDate,
    string? Notes,
    int Status,
    int Priority,
    int? PickingMethod,
    DateTime? ShippingWindowStartUtc,
    DateTime? ShippingWindowEndUtc,
    string? CancelReason,
    DateTime? CanceledAtUtc,
    DateTime CreatedAtUtc,
    bool IsActive,
    IReadOnlyList<OutboundOrderItemViewModel> Items);

public sealed class OutboundOrderListPageViewModel
{
    public IReadOnlyList<OutboundOrderListItemViewModel> Items { get; init; } = Array.Empty<OutboundOrderListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public OutboundOrderQuery Query { get; init; } = new();
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
}

public sealed class OutboundOrderReleaseViewModel
{
    public int? Priority { get; set; }
    public int? PickingMethod { get; set; }
    public DateTime? ShippingWindowStartUtc { get; set; }
    public DateTime? ShippingWindowEndUtc { get; set; }
}

public sealed class OutboundOrderDetailsPageViewModel
{
    public OutboundOrderDetailViewModel Order { get; init; } = null!;
    public OutboundOrderReleaseViewModel Release { get; init; } = new();
    public IReadOnlyList<SelectListItem> PriorityOptions { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> PickingMethodOptions { get; init; } = Array.Empty<SelectListItem>();
}

