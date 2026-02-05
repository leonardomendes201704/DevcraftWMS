namespace DevcraftWMS.Domain.Entities;

public sealed class OutboundPackage : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid OutboundOrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public string PackageNumber { get; set; } = string.Empty;
    public decimal? WeightKg { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }
    public DateTime PackedAtUtc { get; set; }
    public Guid? PackedByUserId { get; set; }
    public string? Notes { get; set; }

    public OutboundOrder? OutboundOrder { get; set; }
    public Warehouse? Warehouse { get; set; }
    public ICollection<OutboundPackageItem> Items { get; set; } = new List<OutboundPackageItem>();
}
