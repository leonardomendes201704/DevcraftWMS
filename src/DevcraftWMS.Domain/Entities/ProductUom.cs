namespace DevcraftWMS.Domain.Entities;

public sealed class ProductUom : AuditableEntity
{
    public Guid ProductId { get; set; }
    public Guid UomId { get; set; }
    public decimal ConversionFactor { get; set; }
    public bool IsBase { get; set; }

    public Product? Product { get; set; }
    public Uom? Uom { get; set; }
}
