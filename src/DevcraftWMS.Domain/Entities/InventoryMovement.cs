using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class InventoryMovement : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid FromLocationId { get; set; }
    public Guid ToLocationId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? LotId { get; set; }
    public decimal Quantity { get; set; }
    public string? Reason { get; set; }
    public string? Reference { get; set; }
    public InventoryMovementStatus Status { get; set; } = InventoryMovementStatus.Draft;
    public DateTime PerformedAtUtc { get; set; }

    public Customer? Customer { get; set; }
    public Location? FromLocation { get; set; }
    public Location? ToLocation { get; set; }
    public Product? Product { get; set; }
    public Lot? Lot { get; set; }
}
