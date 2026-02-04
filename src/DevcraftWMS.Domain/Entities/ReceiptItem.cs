namespace DevcraftWMS.Domain.Entities;

public sealed class ReceiptItem : AuditableEntity
{
    public Guid ReceiptId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? LotId { get; set; }
    public Guid LocationId { get; set; }
    public Guid UomId { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal? ExpectedWeightKg { get; set; }
    public decimal? ExpectedVolumeCm3 { get; set; }
    public decimal? ActualWeightKg { get; set; }
    public decimal? ActualVolumeCm3 { get; set; }
    public decimal? WeightDeviationPercent { get; set; }
    public decimal? VolumeDeviationPercent { get; set; }
    public bool IsMeasurementOutOfRange { get; set; }

    public Receipt? Receipt { get; set; }
    public Product? Product { get; set; }
    public Lot? Lot { get; set; }
    public Location? Location { get; set; }
    public Uom? Uom { get; set; }
}
