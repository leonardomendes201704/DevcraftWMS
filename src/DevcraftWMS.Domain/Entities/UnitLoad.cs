using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class UnitLoad : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ReceiptId { get; set; }
    public string SsccInternal { get; set; } = string.Empty;
    public string? SsccExternal { get; set; }
    public UnitLoadStatus Status { get; set; } = UnitLoadStatus.Created;
    public DateTime? PrintedAtUtc { get; set; }
    public string? Notes { get; set; }

    public Warehouse? Warehouse { get; set; }
    public Receipt? Receipt { get; set; }
}
