using DevcraftWMS.DemoMvc.ViewModels.OutboundOrders;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.OutboundPacking;

public sealed class OutboundPackingQueuePageViewModel
{
    public IReadOnlyList<OutboundOrderListItemViewModel> Items { get; init; } = Array.Empty<OutboundOrderListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public OutboundOrderQuery Query { get; init; } = new();
}

public sealed class OutboundPackingDetailsPageViewModel
{
    public OutboundOrderDetailViewModel Order { get; init; } = default!;
    public OutboundPackingSubmitViewModel Packing { get; init; } = new();
    public IReadOnlyList<OutboundPackageSummaryViewModel> PackedPackages { get; init; } = Array.Empty<OutboundPackageSummaryViewModel>();
    public int PackageCount { get; init; } = 1;
}

public sealed class OutboundPackingSubmitViewModel
{
    public List<OutboundPackageInputViewModel> Packages { get; init; } = new();
}

public sealed class OutboundPackageInputViewModel
{
    public string PackageNumber { get; set; } = string.Empty;
    public decimal? WeightKg { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }
    public string? Notes { get; set; }
    public List<OutboundPackageItemInputViewModel> Items { get; init; } = new();
}

public sealed class OutboundPackageItemInputViewModel
{
    public Guid OutboundOrderItemId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string UomCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public sealed class OutboundPackingRequestViewModel
{
    public IReadOnlyList<OutboundPackageRequestViewModel> Packages { get; init; } = Array.Empty<OutboundPackageRequestViewModel>();
}

public sealed class OutboundPackageRequestViewModel
{
    public string PackageNumber { get; set; } = string.Empty;
    public decimal? WeightKg { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<OutboundPackageItemRequestViewModel> Items { get; set; } = Array.Empty<OutboundPackageItemRequestViewModel>();
}

public sealed class OutboundPackageItemRequestViewModel
{
    public Guid OutboundOrderItemId { get; set; }
    public decimal Quantity { get; set; }
}

public sealed class OutboundPackingResponseViewModel
{
    public IReadOnlyList<OutboundPackageSummaryViewModel> Packages { get; init; } = Array.Empty<OutboundPackageSummaryViewModel>();
}

public sealed class OutboundPackageSummaryViewModel
{
    public string PackageNumber { get; set; } = string.Empty;
    public decimal? WeightKg { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }
    public IReadOnlyList<OutboundPackageItemSummaryViewModel> Items { get; set; } = Array.Empty<OutboundPackageItemSummaryViewModel>();
}

public sealed class OutboundPackageItemSummaryViewModel
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string UomCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}
