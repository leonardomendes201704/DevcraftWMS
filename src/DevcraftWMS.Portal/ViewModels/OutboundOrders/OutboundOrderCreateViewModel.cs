using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.ViewModels.OutboundOrders;

public sealed class OutboundOrderCreateViewModel
{
    public Guid? WarehouseId { get; set; }
    public string? OrderNumber { get; set; }
    public string? CustomerReference { get; set; }
    public string? CarrierName { get; set; }
    public DateOnly? ExpectedShipDate { get; set; }
    public string? Notes { get; set; }
    public List<OutboundOrderItemInputViewModel> Items { get; set; } = new();
    public IReadOnlyList<WarehouseOptionDto> Warehouses { get; set; } = Array.Empty<WarehouseOptionDto>();
    public IReadOnlyList<ProductOptionDto> Products { get; set; } = Array.Empty<ProductOptionDto>();
    public IReadOnlyList<UomOptionDto> Uoms { get; set; } = Array.Empty<UomOptionDto>();
}

public sealed class OutboundOrderItemInputViewModel
{
    public Guid? ProductId { get; set; }
    public Guid? UomId { get; set; }
    public decimal? Quantity { get; set; }
    public string? LotCode { get; set; }
    public DateOnly? ExpirationDate { get; set; }
}

