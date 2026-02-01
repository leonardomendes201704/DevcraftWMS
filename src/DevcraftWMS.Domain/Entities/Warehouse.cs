using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class Warehouse : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? Description { get; set; }
    public WarehouseType WarehouseType { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsPickingEnabled { get; set; }
    public bool IsReceivingEnabled { get; set; }
    public bool IsShippingEnabled { get; set; }
    public bool IsReturnsEnabled { get; set; }
    public string? ExternalId { get; set; }
    public string? ErpCode { get; set; }
    public string? CostCenterCode { get; set; }
    public string? CostCenterName { get; set; }
    public TimeOnly? CutoffTime { get; set; }
    public string? Timezone { get; set; }

    public ICollection<WarehouseAddress> Addresses { get; set; } = new List<WarehouseAddress>();
    public ICollection<WarehouseContact> Contacts { get; set; } = new List<WarehouseContact>();
    public ICollection<WarehouseCapacity> Capacities { get; set; } = new List<WarehouseCapacity>();
    public ICollection<Sector> Sectors { get; set; } = new List<Sector>();
}
