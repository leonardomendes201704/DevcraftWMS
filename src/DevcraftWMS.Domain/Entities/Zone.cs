using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class Zone : AuditableEntity
{
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ZoneType ZoneType { get; set; } = ZoneType.Storage;

    public Warehouse? Warehouse { get; set; }
    public ICollection<Location> Locations { get; set; } = new List<Location>();
    public ICollection<ZoneCustomer> CustomerAccesses { get; set; } = new List<ZoneCustomer>();
}
