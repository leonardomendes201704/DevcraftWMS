using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class ReceiptDivergence : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid ReceiptId { get; set; }
    public Guid? InboundOrderId { get; set; }
    public Guid? InboundOrderItemId { get; set; }
    public ReceiptDivergenceType Type { get; set; }
    public string? Notes { get; set; }
    public bool RequiresEvidence { get; set; }

    public Receipt? Receipt { get; set; }
    public InboundOrderItem? InboundOrderItem { get; set; }
    public ICollection<ReceiptDivergenceEvidence> Evidence { get; set; } = new List<ReceiptDivergenceEvidence>();
}
