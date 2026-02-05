namespace DevcraftWMS.Domain.Entities;

public sealed class OutboundShipment : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid OutboundOrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public string DockCode { get; set; } = string.Empty;
    public DateTime? LoadingStartedAtUtc { get; set; }
    public DateTime? LoadingCompletedAtUtc { get; set; }
    public DateTime ShippedAtUtc { get; set; }
    public string? Notes { get; set; }

    public OutboundOrder? OutboundOrder { get; set; }
    public Warehouse? Warehouse { get; set; }
    public ICollection<OutboundShipmentItem> Items { get; set; } = new List<OutboundShipmentItem>();
}
