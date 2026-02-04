using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class PutawayTask : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ReceiptId { get; set; }
    public Guid UnitLoadId { get; set; }
    public PutawayTaskStatus Status { get; set; } = PutawayTaskStatus.Pending;

    public UnitLoad? UnitLoad { get; set; }
    public Receipt? Receipt { get; set; }
    public Warehouse? Warehouse { get; set; }
}
