using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class ReturnItem : AuditableEntity
{
    public Guid ReturnOrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UomId { get; set; }
    public Guid? LotId { get; set; }
    public string? LotCode { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public decimal QuantityExpected { get; set; }
    public decimal QuantityReceived { get; set; }
    public ReturnItemDisposition Disposition { get; set; } = ReturnItemDisposition.Restock;
    public string? DispositionNotes { get; set; }

    public ReturnOrder? ReturnOrder { get; set; }
    public Product? Product { get; set; }
    public Uom? Uom { get; set; }
    public Lot? Lot { get; set; }
}
