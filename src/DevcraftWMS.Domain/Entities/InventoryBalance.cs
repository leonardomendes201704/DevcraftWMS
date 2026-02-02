using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class InventoryBalance : AuditableEntity
{
    public Guid LocationId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? LotId { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public InventoryBalanceStatus Status { get; set; } = InventoryBalanceStatus.Available;

    public Location? Location { get; set; }
    public Product? Product { get; set; }
    public Lot? Lot { get; set; }
}
