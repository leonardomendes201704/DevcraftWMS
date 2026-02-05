namespace DevcraftWMS.Portal.ViewModels.OutboundOrders;

public sealed class OutboundOrderListQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string OrderBy { get; set; } = "CreatedAtUtc";
    public string OrderDir { get; set; } = "desc";
    public Guid? WarehouseId { get; set; }
    public string? OrderNumber { get; set; }
    public int? Status { get; set; }
    public int? Priority { get; set; }
    public DateOnly? CreatedFromUtc { get; set; }
    public DateOnly? CreatedToUtc { get; set; }
    public bool? IsActive { get; set; }
    public bool IncludeInactive { get; set; }
}
