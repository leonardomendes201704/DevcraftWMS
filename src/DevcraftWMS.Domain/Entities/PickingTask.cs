using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class PickingTask : AuditableEntity
{
    public Guid OutboundOrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public PickingTaskStatus Status { get; set; } = PickingTaskStatus.Pending;
    public int Sequence { get; set; }
    public Guid? AssignedUserId { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? Notes { get; set; }

    public OutboundOrder? OutboundOrder { get; set; }
    public Warehouse? Warehouse { get; set; }
    public ICollection<PickingTaskItem> Items { get; set; } = new List<PickingTaskItem>();
}
