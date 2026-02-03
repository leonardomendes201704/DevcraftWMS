using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class Asn : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public string AsnNumber { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public string? DocumentNumber { get; set; }
    public DateOnly? ExpectedArrivalDate { get; set; }
    public string? Notes { get; set; }
    public AsnStatus Status { get; set; } = AsnStatus.Registered;

    public Warehouse? Warehouse { get; set; }
    public ICollection<AsnItem> Items { get; set; } = new List<AsnItem>();
    public ICollection<AsnAttachment> Attachments { get; set; } = new List<AsnAttachment>();
}
