namespace DevcraftWMS.Domain.Entities;

public sealed class WarehouseContact : AuditableEntity
{
    public Guid WarehouseId { get; set; }
    public bool IsPrimary { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public Warehouse? Warehouse { get; set; }
}
