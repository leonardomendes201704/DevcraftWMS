using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class WarehouseCapacity : AuditableEntity
{
    public Guid WarehouseId { get; set; }
    public bool IsPrimary { get; set; }
    public decimal? LengthMeters { get; set; }
    public decimal? WidthMeters { get; set; }
    public decimal? HeightMeters { get; set; }
    public decimal? TotalAreaM2 { get; set; }
    public decimal? TotalCapacity { get; set; }
    public CapacityUnit? CapacityUnit { get; set; }
    public decimal? MaxWeightKg { get; set; }
    public decimal? OperationalArea { get; set; }

    public Warehouse? Warehouse { get; set; }
}
