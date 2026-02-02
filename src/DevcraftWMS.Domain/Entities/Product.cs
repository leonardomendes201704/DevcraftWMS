namespace DevcraftWMS.Domain.Entities;

public sealed class Product : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Ean { get; set; }
    public string? ErpCode { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public Guid BaseUomId { get; set; }
    public Enums.TrackingMode TrackingMode { get; set; } = Enums.TrackingMode.None;
    public decimal? WeightKg { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? VolumeCm3 { get; set; }

    public Uom? BaseUom { get; set; }
    public ICollection<ProductUom> ProductUoms { get; set; } = new List<ProductUom>();
    public ICollection<Lot> Lots { get; set; } = new List<Lot>();
}
