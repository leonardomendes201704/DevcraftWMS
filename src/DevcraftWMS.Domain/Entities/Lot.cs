using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class Lot : AuditableEntity
{
    public Guid ProductId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateOnly? ManufactureDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public LotStatus Status { get; set; } = LotStatus.Available;
    public DateTime? QuarantinedAtUtc { get; set; }
    public string? QuarantineReason { get; set; }

    public Product? Product { get; set; }
}
