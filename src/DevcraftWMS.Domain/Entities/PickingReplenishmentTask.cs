using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class PickingReplenishmentTask : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UomId { get; set; }
    public Guid FromLocationId { get; set; }
    public Guid ToLocationId { get; set; }
    public decimal QuantityPlanned { get; set; }
    public decimal QuantityMoved { get; set; }
    public PickingReplenishmentStatus Status { get; set; } = PickingReplenishmentStatus.Pending;
    public Guid? AssignedUserId { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? Notes { get; set; }

    public Warehouse? Warehouse { get; set; }
    public Product? Product { get; set; }
    public Uom? Uom { get; set; }
    public Location? FromLocation { get; set; }
    public Location? ToLocation { get; set; }
}
