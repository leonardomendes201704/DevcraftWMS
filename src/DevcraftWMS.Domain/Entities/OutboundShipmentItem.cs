namespace DevcraftWMS.Domain.Entities;

public sealed class OutboundShipmentItem : AuditableEntity
{
    public Guid OutboundShipmentId { get; set; }
    public Guid OutboundPackageId { get; set; }
    public string PackageNumber { get; set; } = string.Empty;
    public decimal? WeightKg { get; set; }

    public OutboundShipment? OutboundShipment { get; set; }
    public OutboundPackage? OutboundPackage { get; set; }
}
