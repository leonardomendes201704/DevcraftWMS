namespace DevcraftWMS.Domain.Entities;

public sealed class PickingTaskItem : AuditableEntity
{
    public Guid PickingTaskId { get; set; }
    public Guid OutboundOrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UomId { get; set; }
    public Guid? LotId { get; set; }
    public Guid? LocationId { get; set; }
    public decimal QuantityPlanned { get; set; }
    public decimal QuantityPicked { get; set; }

    public PickingTask? PickingTask { get; set; }
    public OutboundOrderItem? OutboundOrderItem { get; set; }
    public Product? Product { get; set; }
    public Uom? Uom { get; set; }
    public Lot? Lot { get; set; }
    public Location? Location { get; set; }
}
