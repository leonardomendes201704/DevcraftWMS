using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class Sector : AuditableEntity
{
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SectorType SectorType { get; set; } = SectorType.Storage;

    public Warehouse? Warehouse { get; set; }
}
