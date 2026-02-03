using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class GateCheckin : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid? InboundOrderId { get; set; }
    public string? DocumentNumber { get; set; }
    public string VehiclePlate { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string? CarrierName { get; set; }
    public DateTime ArrivalAtUtc { get; set; }
    public string? DockCode { get; set; }
    public DateTime? DockAssignedAtUtc { get; set; }
    public GateCheckinStatus Status { get; set; } = GateCheckinStatus.CheckedIn;
    public string? Notes { get; set; }

    public InboundOrder? InboundOrder { get; set; }
}
