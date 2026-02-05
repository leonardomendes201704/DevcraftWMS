namespace DevcraftWMS.Domain.Entities;

public sealed class OutboundPackageItem : AuditableEntity
{
    public Guid OutboundPackageId { get; set; }
    public Guid OutboundOrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UomId { get; set; }
    public decimal Quantity { get; set; }

    public OutboundPackage? OutboundPackage { get; set; }
    public OutboundOrderItem? OutboundOrderItem { get; set; }
    public Product? Product { get; set; }
    public Uom? Uom { get; set; }
}
