using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class ReceiptCount : AuditableEntity
{
    public Guid ReceiptId { get; set; }
    public Guid InboundOrderItemId { get; set; }
    public decimal ExpectedQuantity { get; set; }
    public decimal CountedQuantity { get; set; }
    public decimal Variance { get; set; }
    public ReceiptCountMode Mode { get; set; } = ReceiptCountMode.Blind;
    public string? Notes { get; set; }

    public Receipt? Receipt { get; set; }
    public InboundOrderItem? InboundOrderItem { get; set; }
}
