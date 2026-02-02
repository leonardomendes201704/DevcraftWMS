using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class Uom : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public UomType Type { get; set; } = UomType.Unit;

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<ProductUom> ProductUoms { get; set; } = new List<ProductUom>();
}
