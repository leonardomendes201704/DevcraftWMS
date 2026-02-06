using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class ReturnOrder : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? OutboundOrderId { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public ReturnStatus Status { get; set; } = ReturnStatus.Draft;
    public string? Notes { get; set; }
    public DateTime? ReceivedAtUtc { get; set; }
    public Guid? ReceivedByUserId { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public Guid? CompletedByUserId { get; set; }

    public Warehouse? Warehouse { get; set; }
    public OutboundOrder? OutboundOrder { get; set; }
    public ICollection<ReturnItem> Items { get; set; } = new List<ReturnItem>();
}
