using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class InboundOrder : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid AsnId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public string? DocumentNumber { get; set; }
    public DateOnly? ExpectedArrivalDate { get; set; }
    public string? Notes { get; set; }
    public InboundOrderStatus Status { get; set; } = InboundOrderStatus.Issued;
    public InboundOrderPriority Priority { get; set; } = InboundOrderPriority.Normal;
    public InboundOrderInspectionLevel InspectionLevel { get; set; } = InboundOrderInspectionLevel.None;
    public string? SuggestedDock { get; set; }
    public string? CancelReason { get; set; }
    public DateTime? CanceledAtUtc { get; set; }
    public Guid? CanceledByUserId { get; set; }

    public Warehouse? Warehouse { get; set; }
    public Asn? Asn { get; set; }
    public ICollection<InboundOrderItem> Items { get; set; } = new List<InboundOrderItem>();
    public ICollection<InboundOrderStatusEvent> StatusEvents { get; set; } = new List<InboundOrderStatusEvent>();
}
