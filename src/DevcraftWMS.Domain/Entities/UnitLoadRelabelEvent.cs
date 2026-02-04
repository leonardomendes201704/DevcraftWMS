namespace DevcraftWMS.Domain.Entities;

public sealed class UnitLoadRelabelEvent : AuditableEntity
{
    public Guid UnitLoadId { get; set; }
    public Guid CustomerId { get; set; }
    public string OldSsccInternal { get; set; } = string.Empty;
    public string NewSsccInternal { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime RelabeledAtUtc { get; set; }

    public UnitLoad? UnitLoad { get; set; }
}
