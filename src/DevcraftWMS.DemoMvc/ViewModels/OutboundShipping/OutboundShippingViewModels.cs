using DevcraftWMS.DemoMvc.ViewModels.OutboundOrders;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.OutboundShipping;

public sealed class OutboundShippingQueuePageViewModel
{
    public IReadOnlyList<OutboundOrderListItemViewModel> Items { get; init; } = Array.Empty<OutboundOrderListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public OutboundOrderQuery Query { get; init; } = new();
}

public sealed class OutboundShippingDetailsPageViewModel
{
    public OutboundOrderDetailViewModel Order { get; init; } = null!;
    public IReadOnlyList<OutboundPackageShippingViewModel> Packages { get; init; } = Array.Empty<OutboundPackageShippingViewModel>();
    public OutboundShippingSubmitViewModel Shipping { get; init; } = new();
    public IReadOnlyList<OutboundShipmentItemSummaryViewModel> LastShipment { get; init; } = Array.Empty<OutboundShipmentItemSummaryViewModel>();
}

public sealed class OutboundPackageShippingViewModel
{
    public Guid OutboundPackageId { get; init; }
    public string PackageNumber { get; init; } = string.Empty;
    public decimal? WeightKg { get; init; }
    public IReadOnlyList<OutboundPackageShippingItemViewModel> Items { get; init; } = Array.Empty<OutboundPackageShippingItemViewModel>();
}

public sealed class OutboundPackageShippingItemViewModel
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UomCode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
}

public sealed class OutboundShippingSubmitViewModel
{
    public string DockCode { get; set; } = string.Empty;
    public DateTime? LoadingStartedAtUtc { get; set; }
    public DateTime? LoadingCompletedAtUtc { get; set; }
    public DateTime? ShippedAtUtc { get; set; }
    public string? Notes { get; set; }
    public List<OutboundShipmentPackageInputViewModel> Packages { get; set; } = new();
}

public sealed class OutboundShipmentPackageInputViewModel
{
    public Guid OutboundPackageId { get; set; }
    public string PackageNumber { get; set; } = string.Empty;
    public bool Selected { get; set; }
}

public sealed record OutboundShipmentRequestViewModel(
    string DockCode,
    DateTime? LoadingStartedAtUtc,
    DateTime? LoadingCompletedAtUtc,
    DateTime? ShippedAtUtc,
    string? Notes,
    IReadOnlyList<OutboundShipmentPackageRequestViewModel> Packages);

public sealed record OutboundShipmentPackageRequestViewModel(
    Guid OutboundPackageId);

public sealed class OutboundShipmentItemSummaryViewModel
{
    public string PackageNumber { get; init; } = string.Empty;
    public decimal? WeightKg { get; init; }
}
