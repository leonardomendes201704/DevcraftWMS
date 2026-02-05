namespace DevcraftWMS.Domain.Entities;

public sealed class OutboundOrderItem : AuditableEntity
{
    public Guid OutboundOrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UomId { get; set; }
    public decimal Quantity { get; set; }
    public string? LotCode { get; set; }
    public DateOnly? ExpirationDate { get; set; }

    public OutboundOrder? OutboundOrder { get; set; }
    public Product? Product { get; set; }
    public Uom? Uom { get; set; }
}
