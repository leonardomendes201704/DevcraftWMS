using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class DockSchedule : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public string DockCode { get; set; } = string.Empty;
    public DateTime SlotStartUtc { get; set; }
    public DateTime SlotEndUtc { get; set; }
    public DockScheduleStatus Status { get; set; } = DockScheduleStatus.Scheduled;
    public Guid? OutboundOrderId { get; set; }
    public Guid? OutboundShipmentId { get; set; }
    public string? Notes { get; set; }
    public string? RescheduleReason { get; set; }

    public Warehouse? Warehouse { get; set; }
    public OutboundOrder? OutboundOrder { get; set; }
    public OutboundShipment? OutboundShipment { get; set; }
}
