using DevcraftWMS.DemoMvc.ViewModels.OutboundOrders;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.OutboundChecks;

public sealed class OutboundCheckQueuePageViewModel
{
    public IReadOnlyList<OutboundOrderListItemViewModel> Items { get; init; } = Array.Empty<OutboundOrderListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public OutboundOrderQuery Query { get; init; } = new();
}

public sealed class OutboundCheckDetailsPageViewModel
{
    public OutboundOrderDetailViewModel Order { get; init; } = default!;
    public OutboundCheckSubmitViewModel Check { get; init; } = new();
}

public sealed class OutboundCheckSubmitViewModel
{
    public List<OutboundCheckItemInputViewModel> Items { get; init; } = new();
    public string? Notes { get; set; }
}

public sealed class OutboundCheckItemInputViewModel
{
    public Guid OutboundOrderItemId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string UomCode { get; set; } = string.Empty;
    public decimal QuantityExpected { get; set; }
    public decimal QuantityChecked { get; set; }
    public string? DivergenceReason { get; set; }
    public IFormFile? EvidenceFile { get; set; }
}

public sealed class OutboundCheckRequestViewModel
{
    public IReadOnlyList<OutboundCheckItemRequestViewModel> Items { get; init; } = Array.Empty<OutboundCheckItemRequestViewModel>();
    public string? Notes { get; set; }
}

public sealed class OutboundCheckItemRequestViewModel
{
    public Guid OutboundOrderItemId { get; set; }
    public decimal QuantityChecked { get; set; }
    public string? DivergenceReason { get; set; }
    public IReadOnlyList<OutboundCheckEvidenceRequestViewModel>? Evidence { get; set; }
}

public sealed class OutboundCheckEvidenceRequestViewModel
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
}
