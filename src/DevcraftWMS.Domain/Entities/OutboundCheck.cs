using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class OutboundCheck : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid OutboundOrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public OutboundCheckStatus Status { get; set; } = OutboundCheckStatus.Pending;
    public OutboundOrderPriority Priority { get; set; } = OutboundOrderPriority.Normal;
    public Guid? StartedByUserId { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public Guid? CheckedByUserId { get; set; }
    public DateTime? CheckedAtUtc { get; set; }
    public string? Notes { get; set; }

    public OutboundOrder? OutboundOrder { get; set; }
    public Warehouse? Warehouse { get; set; }
    public ICollection<OutboundCheckItem> Items { get; set; } = new List<OutboundCheckItem>();
}
