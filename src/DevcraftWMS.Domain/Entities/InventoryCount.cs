using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class InventoryCount : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid LocationId { get; set; }
    public Guid? ZoneId { get; set; }
    public InventoryCountStatus Status { get; set; } = InventoryCountStatus.Draft;
    public string? Notes { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public Guid? StartedByUserId { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public Guid? CompletedByUserId { get; set; }

    public Warehouse? Warehouse { get; set; }
    public Location? Location { get; set; }
    public Zone? Zone { get; set; }
    public ICollection<InventoryCountItem> Items { get; set; } = new List<InventoryCountItem>();
}
