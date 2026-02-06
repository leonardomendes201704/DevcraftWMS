namespace DevcraftWMS.Domain.Entities;

public sealed class OutboundOrderReservation : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid OutboundOrderId { get; set; }
    public Guid OutboundOrderItemId { get; set; }
    public Guid InventoryBalanceId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? LotId { get; set; }
    public decimal QuantityReserved { get; set; }

    public OutboundOrder? OutboundOrder { get; set; }
    public OutboundOrderItem? OutboundOrderItem { get; set; }
    public InventoryBalance? InventoryBalance { get; set; }
}
