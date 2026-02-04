using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class QualityInspection : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ReceiptId { get; set; }
    public Guid? ReceiptItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? LotId { get; set; }
    public Guid LocationId { get; set; }
    public QualityInspectionStatus Status { get; set; } = QualityInspectionStatus.Pending;
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? DecisionNotes { get; set; }
    public DateTime? DecisionAtUtc { get; set; }
    public Guid? DecisionByUserId { get; set; }

    public Receipt? Receipt { get; set; }
    public ReceiptItem? ReceiptItem { get; set; }
    public Product? Product { get; set; }
    public Lot? Lot { get; set; }
    public Location? Location { get; set; }
    public ICollection<QualityInspectionEvidence> Evidence { get; set; } = new List<QualityInspectionEvidence>();
}
