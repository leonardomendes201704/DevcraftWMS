namespace DevcraftWMS.Domain.Entities;

public sealed class WarehouseAddress : AuditableEntity
{
    public Guid WarehouseId { get; set; }
    public bool IsPrimary { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string? District { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = "BR";
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public Warehouse? Warehouse { get; set; }
}
