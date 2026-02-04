using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class InboundOrderStatusEvent : AuditableEntity
{
    public Guid InboundOrderId { get; set; }
    public InboundOrderStatus FromStatus { get; set; }
    public InboundOrderStatus ToStatus { get; set; }
    public string? Notes { get; set; }

    public InboundOrder? InboundOrder { get; set; }
}
