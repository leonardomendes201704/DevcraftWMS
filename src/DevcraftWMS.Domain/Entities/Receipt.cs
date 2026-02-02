using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class Receipt : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public string? SupplierName { get; set; }
    public ReceiptStatus Status { get; set; } = ReceiptStatus.Draft;
    public DateTime? ReceivedAtUtc { get; set; }
    public string? Notes { get; set; }

    public Warehouse? Warehouse { get; set; }
    public ICollection<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
}
