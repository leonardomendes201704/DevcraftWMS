namespace DevcraftWMS.Domain.Entities;

public sealed class OutboundCheckItem : AuditableEntity
{
    public Guid OutboundCheckId { get; set; }
    public Guid OutboundOrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UomId { get; set; }
    public decimal QuantityExpected { get; set; }
    public decimal QuantityChecked { get; set; }
    public string? DivergenceReason { get; set; }

    public OutboundCheck? OutboundCheck { get; set; }
    public OutboundOrderItem? OutboundOrderItem { get; set; }
    public Product? Product { get; set; }
    public Uom? Uom { get; set; }
    public ICollection<OutboundCheckEvidence> Evidence { get; set; } = new List<OutboundCheckEvidence>();
}
