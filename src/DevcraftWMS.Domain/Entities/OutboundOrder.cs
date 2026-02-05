using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class OutboundOrder : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string? CustomerReference { get; set; }
    public string? CarrierName { get; set; }
    public DateOnly? ExpectedShipDate { get; set; }
    public string? Notes { get; set; }
    public OutboundOrderStatus Status { get; set; } = OutboundOrderStatus.Registered;
    public OutboundOrderPriority Priority { get; set; } = OutboundOrderPriority.Normal;
    public OutboundOrderPickingMethod? PickingMethod { get; set; }
    public DateTime? ShippingWindowStartUtc { get; set; }
    public DateTime? ShippingWindowEndUtc { get; set; }
    public string? CancelReason { get; set; }
    public DateTime? CanceledAtUtc { get; set; }
    public Guid? CanceledByUserId { get; set; }

    public Warehouse? Warehouse { get; set; }
    public ICollection<OutboundOrderItem> Items { get; set; } = new List<OutboundOrderItem>();
}
