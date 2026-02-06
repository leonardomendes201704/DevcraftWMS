namespace DevcraftWMS.Domain.Entities;

public sealed class InventoryCountItem : AuditableEntity
{
    public Guid InventoryCountId { get; set; }
    public Guid LocationId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UomId { get; set; }
    public Guid? LotId { get; set; }
    public decimal QuantityExpected { get; set; }
    public decimal QuantityCounted { get; set; }

    public InventoryCount? InventoryCount { get; set; }
    public Location? Location { get; set; }
    public Product? Product { get; set; }
    public Uom? Uom { get; set; }
    public Lot? Lot { get; set; }
}
